# execute machine learning to enhance UMA dna generation

import os
import tensorflow as tf
import numpy as np
import matplotlib.pyplot as plt
from tensorflow import keras


def runML(data):
    BATCH_SIZE = 20
    BUFFER_SIZE = 50
    EPOCHES = 501
    SNAP_EPOCHES = 50  # how many epoches to print snapshot
    SNAP_N = 25  # how many random dnas are generated IN SNAPSHOT

    test_data = np.array(data)
    test_data = test_data.astype("float32")
    test_data = (test_data-0.5) * 2  # normalize between -1 and 1

    train_dataset = tf.data.Dataset.from_tensor_slices(test_data)
    train_dataset = train_dataset.shuffle(BUFFER_SIZE)
    train_dataset = train_dataset.batch(BATCH_SIZE)

    class Generator(keras.Model):

        def __init__(self, random_noise_size=100):
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

    @tf.function()
    def training_step(generator: Discriminator, discriminator: Discriminator, images: np.ndarray, k: int = 1, batch_size=32):
        for _ in range(k):
            with tf.GradientTape() as gen_tape, tf.GradientTape() as disc_tape:
                noise = generator.generate_noise(batch_size, 100)
                g_z = generator(noise)
                d_x_true = discriminator(images)  # Trainable?
                d_x_fake = discriminator(g_z)  # dx_of_gx

                discriminator_loss = discriminator_objective(
                    d_x_true, d_x_fake)
                # Adjusting Gradient of Discriminator
                gradients_of_discriminator = disc_tape.gradient(
                    discriminator_loss, discriminator.trainable_variables)
                # Takes a list of gradient and variables pairs
                discriminator_optimizer.apply_gradients(
                    zip(gradients_of_discriminator, discriminator.trainable_variables))

                generator_loss = generator_objective(d_x_fake)
                # Adjusting Gradient of Generator
                gradients_of_generator = gen_tape.gradient(
                    generator_loss, generator.trainable_variables)
                generator_optimizer.apply_gradients(
                    zip(gradients_of_generator, generator.trainable_variables))

    def training(dataset, epoches):
        dnas = {}
        for epoch in range(epoches):
            for batch in dataset:
                training_step(generator, discriminator, batch,
                              batch_size=BATCH_SIZE, k=1)

            # After ith epoch generate some dnas
            if (epoch % SNAP_EPOCHES) == 0:
                epochDNAs = []
                for _ in range(SNAP_N):
                    # generating some noise for the training
                    seed = np.random.uniform(-1, 1, size=(1, 100))
                    fake_dna = generator(seed).numpy()[0]
                    fake_dna = fake_dna/2 + 0.5  # un-normalize
                    fake_dna = fake_dna.tolist()
                    epochDNAs.append(fake_dna)
                dnas[str(epoch)] = epochDNAs
        return dnas

    return training(train_dataset, EPOCHES)
