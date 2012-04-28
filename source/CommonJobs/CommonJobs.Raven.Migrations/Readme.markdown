# Consideraciones generales para las migraciones

* Implementar el Down Siempre que sea posible y que tenga sentido. 
* Si el Down o el Up implican perdida de datos, almacenar dichos datos en un campo de backup en la metadata del documento y tener dicha información en cuenta en caso de aplicar la operación inversa (Up o Down respectivamenet).
* Up y Down tienen que permitir una multiple ejecución de los mismos sin que ello implique corrupción de los datos.