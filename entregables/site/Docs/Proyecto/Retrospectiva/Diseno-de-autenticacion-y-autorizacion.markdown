# Diseño de autenticación y autorizacion (retrospectiva)

Si bien este es solo uno más de los aspectos del diseño de la aplicación, creemos que se justifica su inclusión en la sección de retrospectiva ya que fue puesto a prueba antes de terminar el desarrollo: una vez terminada y funcionando la primer implementación basada en _Active Directory_ se requirió un cambio drástico en el comportamiento al migrar a _Google Authentication_. 

Como se explica en el documento sobre el [Diseño de autenticación y autorización](../manual-de-sistema/diseno-autenticacion-y-autorizacion#Migraci%C3%B3n_a_Google_Auth_%2f_usuarios_en_base_de_datos), el buen diseño modular realizado permitió el cambio agregando nuevas clases y modificando la inicialización del sistema respetando el [principio de Open/Closed](http://en.wikipedia.org/wiki/Open/closed_principle).

