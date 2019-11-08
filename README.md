# Neural UMA

Projeto para geração de personagens de jogos digitais através de aprendizado de máquina.
Esse repositório inclui um projeto de Unity, contendo os apps de coleta de dados e o visualizador de treinamento e também o modelo de aprendizagem de máquina escrito em Python, utilizando Tensorflow 2.0

## Instruções de uso

As instruções abaixo são referentes à parte de aprendizado de máquina, contida na pasta ML.

### Pré-requisitos

* Python 3.7
* pipenv
* Nvidia Cuda 10.0

### Instalando

Python pode ser obtido em: https://www.python.org/downloads/

Para instalar o pipenv no Windows: pip3 install pipenv

Cuda 10.0 pode ser baixado em: https://developer.nvidia.com/cuda-10.0-download-archive


Para configurar o ambiente de desenvolvimento, acessar a pasta ML:
```
cd ML
```

E executar o código abaixo, que baixa todas as dependências e cria um ambiente virtual:
```
pipenv install
```

### Parâmetros

As configurações do modelo estão no arquivo machineLearning.py

* EPOCHES: número de etapas de treinamento a serem executadas
* SNAP_EPOCHES: de quantas em quantas épocas são salvas as "fotografias" do aprendizado
* SNAP_N: quantos DNAs são gerados em cada "fotografia"
* FINAL_N: quantos DNAs são gerados ao final do treinamento

### Rodando o modelo

Para rodar o modelo, executar na pasta ML:
```
pipenv run python main.py
```

Esse código irá:
* coletar todos os dados de treinamento da banco de dados remoto (MongoDB)
* executar o treinamento para os personagens masculinos e para os personagens femininos
* enviar os DNAs de cada "fotografia" para o banco de dados remoto
* enviar os DNAs finais do treinamento para o banco de dados remoto