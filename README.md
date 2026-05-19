# Laberinto con Tiempo

Proyecto desarrollado en Unity utilizando Firebase Firestore para la recolección y análisis de datos de jugadores.

## Integrantes

- Angel Gabriel Quintero Morales
- Angely Parra Vallejo

---

# Descripción del Juego

El jugador debe recorrer un laberinto y encontrar la salida antes de que se acabe el tiempo.

Durante la partida se recopilan analíticos sobre el comportamiento del jugador para posteriormente analizarlos en un dashboard conectado con Firebase.

---

# Características

- Sistema de temporizador.
- Laberinto con salas fijas.
- Ranking global de jugadores.
- Recolección de métricas de comportamiento.
- Dashboard conectado a Firebase.
- Guardado de sesiones en Firestore.

---

# Flujo del Juego

## Inicio
El jugador ingresa un nombre o alias.

## Juego
Se recopilan métricas silenciosamente mientras el jugador explora el laberinto.

## Final
Cuando la partida termina:
- Se muestra el puntaje final.
- Se guarda la sesión en Firebase.
- Se consulta el ranking global.

---

# Tecnologías Utilizadas

- Unity
- C#
- Firebase
- Firebase Firestore

---

# Estructura de Firebase

## Colección `sessions`

Guarda toda la información analítica de cada sesión.

## Colección `highscores`

Guarda:
- Nombre del jugador
- Puntaje final

---

# Métricas Implementadas

- Tiempo entre inicio y salida.
- Caminos incorrectos.
- Colisiones con obstáculos.
- Tiempo restante al finalizar.
- Tiempo promedio de decisión.

---

# Dashboard

El dashboard permite visualizar:
- Ranking global.
- Distribución de puntajes.
- Comparación de tiempos.
- Caminos incorrectos por sesión.

El link para entrar a la página web del dashboard es: https://dashboard-web-flax-pi.vercel.app/ 
Además para información adicional sobre la realización y funcionamiento del dashboard, puede ingresar a este repositorio: https://github.com/AngelQuinteroDev/Dashboard-Web
---

# Documentación

## Informe Técnico
Para leer el informe completo del proceso y análisis del proyecto:

➡️ [Ver INFORME.md](./INFORME.md)

---

# Video

Link del video de funcionamiento:

[Agregar enlace aquí]
