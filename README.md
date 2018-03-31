K-Nearest Neighbor(O vizinho mais próximo), é um algoritmo Usado para classificar objetos com base em exemplos de treinamento que estão mais próximos no espaço de características.

Este foi um trabalho para a disciplina Introdução a Business Intelligence e Datawarehouse do curso Tecnologia em Análise e Desenvolvimento de Sistemas e desenvolvido em c#, o programa tem as seguintes funcionalidades:

- Recebe 3 parâmetros
1. K (o número de vizinhos mais próximos que serão considerados pelo algoritmo )
2. Arquivo com os dados a serem classificados.
3. Arquivo com os dados de treinamento.
- Para cada linha do arquivo a ser classificado calcula a distância Euclidiana para os dados no arquivo de treinamento.
- Classifica as entradas do primeiro arquivo
- Mostra a Matriz de Confusão
