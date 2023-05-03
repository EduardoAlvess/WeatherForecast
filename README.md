# WeatherForecast

Desafio de criar uma API de previsão do tempo a partir do CEP, utilizando MongoDB, ElasticSearch e Docker.

Desafio:
Criar uma Api retornando JSON integrando com duas APIs públicas e gratuitas.
Passo a passo:
• Crie um banco de dados MongoDB em Docker.
• Crie um ElasticSearch em Docker.
• Crie uma API.
• Crie um endpoint na sua api para criar usuário e senha e salvar no mongoDB.
• Crie um endpoint na sua api com o método "POST" com um campo CEP requirido, onde será consultado o endereço usando o CEP na api da ViaCEP (https://viacep.com.br/) e pegar a cidade retornada e buscar a previsão do tempo dos 4 dias dessa cidade na api do INPE (http://servicos.cptec.inpe.br/XML/). OBS: A sua api deve retornar todos os campos da ViaCEP e do INPE juntos em JSON.
• Crie logs de todas as requisições feita na sua api e salve no ElasticSearch.
• Crie um endpoint na sua api com o método "GET" para trazer todos os logs do usuário.
• Crie uma documentação utilizando Swagger que fique disponível no endpoint "/docs".
• Crie um arquivo Dockfile e docker-compose.yaml para rodar container da api.
