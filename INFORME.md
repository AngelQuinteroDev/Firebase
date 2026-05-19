# Informe – Sistema de Analíticos para “Laberinto con Tiempo”

## Introducción

Cuando recibimos la temática de “Laberinto con tiempo”, inicialmente pensamos en desarrollar únicamente la mecánica básica de recorrer un mapa antes de que se agotara el temporizador, sin embargo, al revisar los requisitos del proyecto entendimos que el objetivo principal no era hacer un juego complejo, sino construir un sistema capaz de recopilar información útil sobre cómo juegan las personas.

A partir de eso, comenzamos a preguntarnos qué cosas realmente valía la pena medir dentro de un laberinto; el uso de la base de datos no solo sería para guardar únicamente puntajes, porque eso no explicaba el comportamiento del jugador, nos interesaba entender si el jugador se perdía, si dudaba demasiado en ciertas zonas o si el tiempo límite realmente era justo, teniendo en cuenta la dificultad del laberinto.

Con esa idea empezamos a diseñar tanto el juego como el sistema de analíticos al mismo tiempo.

---

# Diseño de la Mecánica

La mecánica principal consiste en que el jugador debe atravesar un laberinto y encontrar la salida antes de que el tiempo llegue a cero. El mapa está compuesto por salas fijas y caminos cerrados que obligan al jugador a tomar decisiones constantemente.

Durante las primeras pruebas notamos algo importante: aunque algunos jugadores lograban llegar rápido a la salida, otros pasaban demasiado tiempo dudando en las intersecciones, con eso, entendimos que el tiempo final no era suficiente para analizar la experiencia completa.

A partir de ahí empezamos a definir métricas más específicas.

---

# Diseño del Sistema de Analíticos

Decidimos usar Firebase Firestore porque necesitábamos una base de datos flexible y fácil de conectar con Unity, además, nos permitía guardar información de cada sesión sin necesidad de crear un backend propio.

Al principio pensamos guardar datos constantemente mientras el jugador avanzaba por el laberinto, pero nos dimos cuenta de que eso generaba demasiadas escrituras innecesarias en Firebase y podría afectar el rendimiento del juego.

Por esa razón decidimos almacenar temporalmente toda la información durante la partida y enviar los datos a Firestore únicamente cuando la sesión terminara, ya fuera porque el jugador llegó a la meta o porque el tiempo se agotó.

Esta decisión hizo el sistema más eficiente, redujo la cantidad de operaciones en la base de datos y facilitó la organización de cada sesión como un único documento completo dentro de la colección `sessions`.

---

# Métricas Implementadas

## Tiempo entre inicio y salida

Esta fue la primera métrica que definimos porque representa directamente el objetivo principal del juego.

Nos permitió identificar:
- Qué tan rápido resolvían el laberinto los jugadores.
- Si el mapa era demasiado sencillo o demasiado difícil.
- Cuánto influía la experiencia previa del jugador.

Durante las pruebas vimos diferencias muy grandes entre sesiones, lo que confirmó que esta métrica era importante para medir habilidad y dificultad.

---

## Cuenta de pausa
Después agregamos una métrica para determinar cuantas veces un jugador pausa el tiempo. Esto es importante para determinar el comportamiento de los jugadores
---

## Colisiones con paredes u obstáculos

También decidimos registrar las colisiones del jugador.

Inicialmente pensamos que no sería una métrica muy relevante, pero durante las pruebas notamos que las colisiones aumentaban mucho cuando los jugadores entraban en pánico por el tiempo restante.

Eso convirtió esta métrica en un indicador indirecto de estrés y dificultad.

---

## Tiempo restante al finalizar

Otra métrica importante fue el tiempo sobrante cuando el jugador llegaba a la meta.

Queríamos saber si el temporizador estaba bien balanceado. Si casi todos terminaban con demasiado tiempo, el reto perdía tensión. Pero si casi nadie lograba terminar, el juego se volvía frustrante.

Esta métrica ayudó a ajustar mejor la duración de las partidas.

---

# Métrica de Comportamiento

## Tiempo promedio de decisión

La métrica más interesante fue el tiempo promedio de decisión en intersecciones.

La implementamos porque nos dimos cuenta de que muchos jugadores se detenían antes de elegir un camino. Esto no se reflejaba en el puntaje ni en el tiempo total, pero sí mostraba dudas y confusión.

Con esta métrica podíamos identificar:
- Qué zonas del laberinto generaban más incertidumbre.
- Qué tan intuitivo era el diseño.
- Cómo reaccionaban distintos jugadores bajo presión.

Fue probablemente el analítico que más información útil aportó.

---

# Dashboard y Visualización

Una vez teníamos datos suficientes, desarrollamos un dashboard separado del juego.

Queríamos que las estadísticas fueran fáciles de interpretar visualmente, así que incluimos:
- Ranking global de puntajes.
- Distribución de tiempos.
- Comparación de caminos incorrectos entre sesiones.
- Relación entre tiempo restante y puntaje final.

Lo más interesante fue observar patrones repetidos, por ejemplo, algunos jugadores tenían buenos tiempos finales pero muchísimos caminos incorrectos, mientras que otros jugaban más lento pero de forma mucho más eficiente.

---

# Problemas Encontrados

Uno de los problemas principales fue organizar correctamente las sesiones dentro de Firebase.

Al comienzo algunas partidas sobrescribían datos porque no estábamos generando identificadores únicos correctamente, esto se solucionó dejando que Firestore generara automáticamente el ID de cada documento.

Otro problema fue decidir qué datos realmente valía la pena guardar, había muchas variables posibles, pero entendimos que guardar demasiada información también hacía más difícil analizarla después.

Por eso intentamos enfocarnos únicamente en métricas que realmente dijeran algo sobre el comportamiento del jugador.

---

# Conclusión

Este proyecto nos permitió entender que los analíticos no consisten solo en almacenar números, sino en interpretar cómo interactúan los jugadores con una mecánica.

Aunque el juego era relativamente sencillo, los datos recopilados mostraron comportamientos muy distintos entre jugadores. Algunas personas priorizaban velocidad, otras exploraban más y otras dudaban constantemente antes de tomar decisiones.

El uso de Firebase y Firestore facilitó la recolección y visualización de toda esta información, y el dashboard permitió convertir los datos en conclusiones útiles para mejorar el diseño del juego.

Más que desarrollar un laberinto, el proyecto terminó siendo una experiencia para aprender cómo los datos pueden ayudar a entender mejor la experiencia de usuario dentro de un videojuego.

---

# Navegación

⬅️ [Volver al README](./README.md)
