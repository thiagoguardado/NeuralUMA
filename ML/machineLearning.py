# execute machine learning to enhance UMA dna generation

import os
import datetime
import tensorflow as tf
import numpy as np
import matplotlib.pyplot as plt
from tensorflow import keras


def runML(data,race):
    NOISE_SIZE = 100
    BATCH_SIZE = 20
    BUFFER_SIZE = 50
    EPOCHES = 501
    SNAP_EPOCHES = 50  # how many epoches to print snapshot
    SNAP_N = 100  # how many random dnas are generated IN SNAPSHOT
    FINAL_N = 5000  # how many random dnas are generated after training complete

    test_data = np.array(data)
    test_data = test_data.astype("float32")
    test_data = (test_data-0.5) * 2  # normalize between -1 and 1

    train_dataset = tf.data.Dataset.from_tensor_slices(test_data)
    train_dataset = train_dataset.shuffle(BUFFER_SIZE)
    train_dataset = train_dataset.batch(BATCH_SIZE)

    class Generator(keras.Model):

        def __init__(self, random_noise_size=NOISE_SIZE):
            super().__init__(name='generator')
            # layers
            self.input_layer = keras.layers.Dense(units=random_noise_size)
            self.dense_1 = keras.layers.Dense(units=128)
            self.leaky_1 = keras.layers.LeakyReLU(alpha=0.01)
            self.dense_2 = keras.layers.Dense(units=128)
            self.leaky_2 = keras.layers.LeakyReLU(alpha=0.01)
            self.dense_3 = keras.layers.Dense(units=128)
            self.leaky_3 = keras.layers.LeakyReLU(alpha=0.01)
            self.output_layer = keras.layers.Dense(
                units=20, activation="tanh")

        def call(self, input_tensor):
            # Definition of Forward Pass
            x = self.input_layer(input_tensor)
            x = self.dense_1(x)
            x = self.leaky_1(x)
            x = self.dense_2(x)
            x = self.leaky_2(x)
            x = self.dense_3(x)
            x = self.leaky_3(x)
            return self.output_layer(x)

        def generate_noise(self, batch_size, random_noise_size):
            return np.random.uniform(-1, 1, size=(batch_size, random_noise_size))

    generator = Generator()

    cross_entropy = tf.keras.losses.BinaryCrossentropy(from_logits=True)

    def generator_objective(dx_of_gx):
        # Labels are true here because generator thinks he produces real images.
        return cross_entropy(tf.ones_like(dx_of_gx), dx_of_gx)

    class Discriminator(keras.Model):
        def __init__(self):
            super().__init__(name="discriminator")

            # Layers
            self.input_layer = keras.layers.Dense(units=20)
            self.dense_1 = keras.layers.Dense(units=128)
            self.leaky_1 = keras.layers.LeakyReLU(alpha=0.01)
            self.dense_2 = keras.layers.Dense(units=256)
            self.leaky_2 = keras.layers.LeakyReLU(alpha=0.01)
            self.dense_3 = keras.layers.Dense(units=128)
            self.leaky_3 = keras.layers.LeakyReLU(alpha=0.01)

            # This neuron tells us if the input is fake or real
            self.logits = keras.layers.Dense(units=1)

        def call(self, input_tensor):
              # Definition of Forward Pass
            x = self.input_layer(input_tensor)
            x = self.dense_1(x)
            x = self.leaky_1(x)
            x = self.leaky_2(x)
            x = self.leaky_3(x)
            x = self.leaky_3(x)
            x = self.logits(x)
            return x

    discriminator = Discriminator()

    def discriminator_objective(d_x, g_z, smoothing_factor=0.9):
        """
        d_x = real output
        g_z = fake output
        """
        real_loss = cross_entropy(tf.ones_like(d_x) * smoothing_factor,
                                  d_x)  # If we feed the discriminator with real images, we assume they all are the right pictures --> Because of that label == 1
        # Each noise we feed in are fakes image --> Because of that labels are 0.
        fake_loss = cross_entropy(tf.zeros_like(g_z), g_z)
        total_loss = real_loss + fake_loss

        return total_loss

    generator_optimizer = keras.optimizers.RMSprop()
    discriminator_optimizer = keras.optimizers.RMSprop()

    # scalar metrics
    metrics_discriminator_loss = tf.keras.metrics.Mean('metrics_discriminator_loss', dtype=tf.float32)
    # metrics_discriminator_accuracy = tf.keras.metrics.SparseCategoricalAccuracy('metrics_discriminator_accuracy')
    metrics_generator_loss = tf.keras.metrics.Mean('metrics_generator_loss', dtype=tf.float32)
    # metrics_generator_accuracy = tf.keras.metrics.SparseCategoricalAccuracy('metrics_generator_accuracy')

    scalarMetricsNamedDir = datetime.datetime.now().strftime("%Y%m%d-%H%M%S") + "-" + race
    discriminator_log_dir = 'logs/gradient_tape/' + scalarMetricsNamedDir + '/discriminator'
    generator_log_dir = 'logs/gradient_tape/' + scalarMetricsNamedDir + '/generator'
    discriminator_summary_writer = tf.summary.create_file_writer(discriminator_log_dir)
    generator_summary_writer = tf.summary.create_file_writer(generator_log_dir)

    # model graphs
    modelGraphsNamedDir = datetime.datetime.now().strftime("%Y%m%d-%H%M%S") + "-" + race
    modelGraphs_log_dir = 'logs/func/' + modelGraphsNamedDir
    modelGraphs_writer = tf.summary.create_file_writer(modelGraphs_log_dir)

    @tf.function()
    def training_step(generator: Discriminator, discriminator: Discriminator, dna: np.ndarray, k: int = 1, batch_size=32):
        for _ in range(k):
            with tf.GradientTape() as gen_tape, tf.GradientTape() as disc_tape:
                noise = generator.generate_noise(batch_size, NOISE_SIZE)
                g_z = generator(noise)
                d_x_true = discriminator(dna)  # Trainable?
                d_x_fake = discriminator(g_z)  # dx_of_gx

                discriminator_loss = discriminator_objective(d_x_true, d_x_fake)
                # Adjusting Gradient of Discriminator
                gradients_of_discriminator = disc_tape.gradient(discriminator_loss, discriminator.trainable_variables)
                discriminator_optimizer.apply_gradients(zip(gradients_of_discriminator, discriminator.trainable_variables))

                # metrics
                metrics_discriminator_loss(discriminator_loss)
                # metrics_discriminator_accuracy(d_x_true,d_x_fake)

                generator_loss = generator_objective(d_x_fake)
                # Adjusting Gradient of Generator
                gradients_of_generator = gen_tape.gradient(generator_loss, generator.trainable_variables)
                generator_optimizer.apply_gradients(zip(gradients_of_generator, generator.trainable_variables))

                # metrics
                metrics_generator_loss(generator_loss)
                # metrics_generator_accuracy(tf.ones_like(d_x_fake),d_x_fake)

    def snapshot(dnasToCreate):
        snapshotDNAs = []
        for _ in range(dnasToCreate):
            # generating some noise for the training
            seed = np.random.uniform(-1, 1, size=(1, NOISE_SIZE))
            fake_dna = generator(seed).numpy()[0]
            fake_dna = fake_dna/2 + 0.5  # un-normalize
            fake_dna = fake_dna.tolist()
            snapshotDNAs.append(fake_dna)
        return snapshotDNAs

    def training(dataset, epoches):
        dnas = {}
        dnas["epoches"] = {}
        firstRun = True
        for epoch in range(epoches):
            for batch in dataset:
                if(firstRun):
                    tf.summary.trace_on(graph=True, profiler=False)

                training_step(generator, discriminator, batch,batch_size=BATCH_SIZE, k=1)

                if(firstRun):
                    with modelGraphs_writer.as_default():
                        tf.summary.trace_export(
                            name="my_func_trace",
                            step=0,
                            profiler_outdir=modelGraphs_log_dir)
                firstRun = False
            # log metrics
            with discriminator_summary_writer.as_default():
                tf.summary.scalar(race+'-loss', metrics_discriminator_loss.result(), step=epoch)
                # tf.summary.scalar('accuracy', metrics_discriminator_accuracy.result(), step=epoch)
            with generator_summary_writer.as_default():
                tf.summary.scalar(race+'-loss', metrics_generator_loss.result(), step=epoch)
                # tf.summary.scalar('accuracy', metrics_generator_accuracy.result(), step=epoch)

            # Reset metrics every epoch
            metrics_discriminator_loss.reset_states()
            metrics_generator_loss.reset_states()
            # metrics_discriminator_accuracy.reset_states()
            # metrics_generator_accuracy.reset_states()

            # After ith epoch generate some dnas
            if (epoch % SNAP_EPOCHES) == 0:
                epochDNAs = snapshot(SNAP_N)
                dnas["epoches"][str(epoch)] = epochDNAs

        # final export
        finalDNAs = snapshot(FINAL_N)
        dnas["final"] = finalDNAs
        
        return dnas

    return training(train_dataset, EPOCHES)