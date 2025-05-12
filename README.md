# Proyecto final de IAV - Implementación Utility AI
> **Marcos Pérez Martínez**

## Instalación y uso

> Todo el contenido del proyecto está disponible aquí en este proyecto. **Unity 2022.3.56f1** es empleado para este proyecto. También es necesario inicializar Git LFS al bajarse el repositorio. 

## Introducción
> Este es el proyectio final de la asignatura de Inteligencia Artificial para Videojuegos del Grado en Desarrollo de Videojuegos de la UCM.
>
> La idea de este proyecto es desarrollar una *Utility AI* funcional desde cero e implementar un comportamiento algo complejo a varios enemigos.

## Punto de partida
> Se parte de un proyecto base proporcionado por el profesor y disponible en este repositorio : [Minotaur - Base](https://github.com/narratech/minotaur-base).
> 
> Sin embargo, hay muchas funcionalidades de la base que se deshechan. Se mantiene la generación del laberinto y de enemigos y el movimiento del jugador. La implementación de grafo para recorrer el laberinto se quita y se sustituye por una *NavMesh* para mayor facilidad de cálculos de rutas en el proyecto. Esto se debe a que la practica en sí está enfocada en la correcta implementación de la *Utility AI* y no en la navegación. También se desecha los scripts de Agente y ComportamientoAgente.

## Planteamiento de problemas 
> ### Utility AI
> Implementar las clases básicas de una *Utility AI* para que sea utilizable para cualquier persona que se descargue el proyecto.
> ### Elementos básicos
> - Tanto el jugador como los enemigos tienen puntos de fuerza (PF). Estos puntos son asignados de forma aleatoria (hasta un máximo de 6 PF) excepto al jugador, que se le asignará 2 PF desde un inicio.
> -  A lo largo del mapa se encontrarán orbes las cuales sumarán 1 PF al jugador cuando pasa sobre ellas.
> -  Los enemigos pueden aparecer solos o acompañados. Cuando están acompañados estan quietos y si están solos hacen patruya de un punto a otro.

> [!NOTE]
> Ya se verá si meto lo de las patruyas o no.

> ### Comportamiento de los enemigos
> - Cuando un **SOLO** enemigo vea al jugador podrá tener dos comportamientos:
>   - **ATACAR**: en caso de tener más PF que le jugador. Emitirá un aviso y si hay algun enemigo cerca irá a acompañarlo. En  caso de estar acompañado.
>   - **HUIR**: en caso de tener menos puntos. En este caso, irá en busca de compañeros.
> 
> - Cuando los enemigos vayan en **GRUPO**, la acción de atarcar o huir se verá determinado por la media aritmética de los PF de todos los enemigos.
>   - **ATACAR** : si pueden, se subdivirán en dos grupos **cuya media no sea más debil que el jugador**. Un grupo atacará al jugador directamente. Otro grupo dará un rodeo para flanquear al jugador y atacarle por la espalda
>   - **HUIR** : en el caso de existir varias rutas de escape, el grupo se subdividirá en dos (de forma que los grupos sean lo más fuerte posible) y cada subgrupo tomará una de las rutas posibles. 
