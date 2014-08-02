# Diseño del sistema (retrospectiva)

## Diseño inicial

Al comenzar el proyecto, teníamos distintos grados de definición para un proyecto del sistema, y la imagen mental del diseño ideal que cada integrante tenía difería de los demás. Sabíamos que queríamos aproximar un sistema a través del patrón arquitectural MVC por las tecnologías involucradas, pero el diseño de integración a componentes, desacoplamiento de los datos o arquitectura del dominio eran indefinidos a ese punto.

A lo largo del proyecto se fueron tomando decisiones que cambió el rumbo del ideal que el sistema perseguía, pero los siguientes aspectos se mantuvieron firmes:

- El dominio debía estar desacoplado de la tecnología de persistencia
- El sistema debía ser una dpeendencia del dominio, no al revés (influencia de DDD y Onion Layer Architectures)
- La comunicación cliente-servidor debía ser estrictamente REST
- En lo posible, cada componente del sistema debía evitar depender de otros componentes

## Resultado final

El resultado final del diseño, si bien cumple con los requisitos impuestos, no provee una visión unificada del sistema. Varios subsistemas se encuentran implementados con ciertas filosofías o prioridades, mientras que otros siguen un camino distinto. Esto no es un problema para el sistema o la integración de dichos componentes (puesto que mantienen un buen desacoplamiento), pero es difícil dar una explicación única como diseño del sistema.

Sentimos que de haber sido analizado con anterioridad podríamos haber generado un diseño consistente y que aún así cumpliera con estos requisitos. Creemos que el resultado actual es consecuencia de la importancia que le dimos a la agilidad del proyecto, el avance y los resultados tangibles, opuestos al diseño interno consistente de un sistema completo.