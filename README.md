# PC7866 Embega test resistivo.

Aplicación windows forms para la implementación de un test resistivo para el modelo PC7866 de Embega, según documento proporcionado por
Embega, "Pliego_Condiciones_Control_Electrico_V2.pdf". 
Esta aplicación es el interfaz entre el sistema de medición y el usuario, permitiendo la configuración de los parámetros de prueba, 
la ejecución del test y la visualización de los resultados.
Una base de datos MariaSQL se utiliza para almacenar los resultados de las pruebas, permitiendo un seguimiento histórico y análisis de 
los datos obtenidos, así como los parámetros de configuración utilizados en cada prueba. Se permitirá la exportación de los resultados 
a formatos como CSV o Excel para su posterior análisis o presentación.

## Tabla de contenido
- [Instalación](#instalación)
- [Uso](#uso)
	- [Interfaz de usuario](#interfaz-de-usuario)
	- [Manual](#modo manual)
	- [Automático](#modo automático)
	- [Resultados](#resultados)


- [Protocolo de comunicación](#protocolo-de-comunicación)
- [Base de datos](#base-de-datos)
	- [Referencias](#referencias)
	- [Resultados](#resultados)

## Instalación

### Requisitos previos



### Para instalar la aplicación, siga estos pasos:



## Uso
### Interfaz de usuario
La interfaz de usuario sigue el diseño clásico de aplicaciones windows genéricas, con un menu superior para acceder 
a las diferentes funcionalidades, y una zona central para la configuración de los parámetros de prueba, la ejecución
del test y la visualización de los resultados. No está pensada como una aplicación "responsive", sin que se ha diseñado para una
resolución FULL-HD. 
En la parte inferior, se muestra una barra de estado para indicar el estado actual de la aplicación, como por ejemplo "Conectado al dispositivo",
"Modo automático", las descripción de las etapas de la prueba, etc.
En resto de pantalla queda definida con un marco modal donde mostraremos:
	
	- Manual. 
	- Automático o modo de trabajo normal. Muestra la imagen de la referencia a ensayar, indicando mediante un punto en gris los no medidos, 
		en rojo los NOK y verde los OK los parámetros de ensayo configurados para dicha referencia, un botón para iniciar el ensayo, 
		una zona de visualización de los resultados detallados del ensayo, y un botón para finalizar el ensayo. 
		Tendrá un campo para introducir el lote, el operario, etc.
	- Informes o resultados, donde poder explorar los resultados de las pruebas realizadas, con opciones de filtrado, búsqueda y exportación de datos.
	- Parámetros: Aquí se configuran las diferentes referencias a ensayar, según la tabla de referencias de la base de datos, y los parámetros de ensayo asociados a cada referencia.
		( Tabla parámetros ensayo) Tendrá tantos campos como la tabla "referencias" (ver más abajo), así como una tabla gráfica.
		En esta tabla gráfica, se mostrarán los parámetros de ensayo asociados a cada referencia, y se podrán modificar o eliminar.
	- Configuración (Accesible desde Archivo - Configuración): Desde aquí se configuran los parámetros de comunicación, como el puerto y la velocidad de comunicación.


El modo manual nos permite realizar una prueba de comunicación, realizar mediciones individuales activando salidas de forma manual, 
y visualizar los resultados de cada medición. Permite seleccionar el puerto de comunicación y la velocidad de comunicación.
Inicialmente este modo se ha utilizado para el desarrollo y la depuración, pero se mantendrá en la aplicación final para permitir pruebas puntuales o de diagnóstico.
El modo automático muestra la visualización normal de la aplicación. En esta visualización tendremos:
 - Configuración de los parámetros de ensayo, como la referencia a ensayar, el lote, el operario, etc.
 - Una imagen (Si se ha configurado en la referencia) de la referencia a ensayar. En esta imagen se mostrará mediante un código de 
colores el resultado en un circulo: Gris para no ensayado, verde para bueno y rojo para malo. 
 - Un botón para iniciar el ensayo, el cual se irá actualizando con la etapa del ensayo que se esté realizando en cada momento.
 - Una zona de visualización de los resultados, donde se mostrarán los resultados detallados de cada paso del ensayo, como por ejemplo la resistencia medida, el resultado OK/NOK, etc.
 - Un botón para finalizar el ensayo, el cual guardará los resultados en la base de datos y permitirá la exportación de los resultados a formatos como CSV o Excel.

## Procolo de comunicación
 Comandos:
- Los comandos serán peticiones de un maestro externo (PC).
- La trama comenzará con un código de comando (1 byte, letras mayúsculas ASCII) seguida de los parámetros dependientes del comando:
	- 0x44 ("D") - Diagnosis
	- 0x53 ("S") - Activación salidas 
	- 0x52 ("R") - Lectura valores ADS RAW
	- 0x46 ("F") - Lectura valores ADS Filtrados
	- 0x49 ("I") - Modificación coeficientes de los filtros.
	- 0x47 ("G") - Grabación de los parámetros en EEPROM
	- 0x51 ("Q") - Reinicio del microcontrolador
			
==== COMANDO D Diagnosis ====
Los parámetros de este comando serán otro byte ASCII:
	- 0x54 ("T") - Diagnosis completa: Estado de la comunciación I2C para cada dirección. 
	- 0x31 ("1") - Diagnosis comunicación I2C para el MCP con dirección 0x20
	- 0x32 ("2") - Diagnosis para 0x21
	- 0x33 ("3") - Diagnosis para 0x22
	- 0x24 ("4") - Diagnosis para 0x48
Se generará una respuesta legible ASCII (Por ejemplo, "Comunicación MCP23017 dirección 0x20 correcta." y terminada con <CR><LF>
			
==== COMANDO S Salidas ====
Los parámetros de este comando serán 4 hex codificados ASCII, desde 0000 hasta FFFF, que representan la codificación de las 16 salidas, incluyendo los ceros a la izquierda.
Según esta codificación, se activarán las salidas correspondientes. Por ejemplo (Los espacios son solo para visualizar los tres números, en la trama no existen):
"0000 0000 0000" - Desactiva todas las salidas. 
"0000 0000 0001" - Todas salidas a OFF excepto la salida 1 a ON.
"0000 0000 0002" - Todas salidas a OFF excepto la salida 2 a ON.
"0000 0000 0004" - Todas salidas a OFF excepto la salida 3 a ON.
	....
"0002 0001 0100" - Todas salidas a OFF, excepto 34, 17 y 8 a ON.
“FFFF FFFF FFFF” – Todas las salidas a ON.
La respuesta a este comando será:
	- 0x4F ("O") - OK - Tras la activación correcta de las salidas.
	- 0x4E ("N") - NOK - Fallo al activar las salidas.			
==== COMANDO R Lectura analógicas RAW ====
No hay parámetros para este comando, el microcontrolador devolverá la última lectura actualizada de los cuatro canales analógicos, codificados como un número en ASCII:
"32767 49439 43245 63567" - Indica que la primera analógica está en valor 0V, la segunda en 4.267V ,...
				
==== COMANDO F Lectura analógicas filtradas ====
Igual que el anterior pero tras pasar los filtros y expresada en voltios. Es la que utilizaremos para la medición. Dado que la resistencia de referencia es de 390Ohm, la fórmula para el cálculo de la resistencia que queremos medir, y teniendo en cuenta una caída óhmica en el transistor de potencia de 2’5 Ohm. a 10 mA:

R=V_ain/(V_e- V_ain )*390 Ohm 
Donde:
Vain = Tensión medida en la resistencia R (Analógica 1)
Ve = Tensión de excitación (Analógica 2)
390 = Valor de resistencia de referencia

	
	
==== COMANDO I Modificación coeficientes filtro notch e IIR y flags ====
Un segundo byte define qué vamos a enviar:
	- 0x30 - FLAGS: 16 variables bool, codificados mediante 5 números ASCII. (0 a FFFF).
	- 0x31 - Coeficiente 1  de los filtros. Inicialmente quedarán en memoria volatil hasta ejecutar un guardado de los parámetros. Coma fija, según necesidad.
	- 0x32 - Coeficiente 2
	- 0x33 - Coeficiente 3 
	Los coeficientes están codificados según la fórmula:
val = (uint16_t)((coef + 2.0) × 10000)
Por ejemplo, un coeficiente de 0.95553, nos daría un valor hexadecimal:
	Val = (0.95553 + 2.0) x 10000 = 29555 = 0x7373
			
	Por ejemplo, para activar el FLAG de log, enviaríamos "I0 0001" (El espacio no se envía, es solo para visualización en este documento) y recibiríamos "O" de OK.
Para modificar el coeficiente 1, suponiendo un coeficiente entre 0 y 1, tendríamos que enviar "I1 45678" para un coeficiente de 0'45678. Devuelve "O" de OK.
También permite modificar "FLAGS" de diagnosis, como por ejemplo las salidas "LOG" en el mismo puerto, las cuales no son solicitadas por el PC.
FLAGS:
	- Bit 0 - Activación LOG
	- Bit 1 - Activación filtros
	- Resto reservados

				
 
==== COMANDO G Guardado de parámetros en memoria no volátil y lectura de estos. ====
Un segundo byte ASCII indicará la operación a realizar:
- 0x47 ("G") - Guardar en EEPROM los parámetros actuales en RAM
- 0x4C ("L") - Leer desde EEPROM los parámetros actuales hacia la RAM
Para los dos comandos anteriores, la respuesta es:
- 0x4F ("O") - OK - Tras el guardado en EEPROM
- 0x4E ("N") - NOK - Fallo en el guardado.
- 0x56 ("V") - Visualizar por el puerto serie los valores actuales en RAM para los FLAGS y los coeficientes.
En este caso la respuesta serán los valores con el mismo formato que el comando I.
					
	- Se empleará ESP-IDF versión V6.0.1 como api de programación, ya que está más optimizada para este microcontrolador.

La comunicación por defecto es 115200bps, y normalmente en el COM4. Este puerto puede variar según el PC y la configuración del mismo. 
Los valores de comunicación en una pantalla de configuración de parámetros.


## Base de datos:
	Se preven dos tablas en la base de datos, una para almacenar los resultados de las pruebas y otra para almacenar los parámetros de 
ensayo de cada referencia.


### Referencias
	La tabla referencias almacenará información sobre cada referencia que se pruebe, incluyendo su nombre, descripción y fechas de creación y modificación.
Además tiene una tabla subornidada para almacenar los parámetros de ensayo asociados a cada referencia, en la que se incluirán
los valores necesarios para hacer el ensayo de dicha referenica.
	- Tabla "Referencias":
		- id (INT, PRIMARY KEY, AUTO_INCREMENT)
		- bActiva (BOOLEAN) - Indica si es la referencia activa. Cada vez que realicemos una modificación de los parametros
de ensayo, se creará una nueva referencia con los nuevos parámetros, y se marcará como activa, mientras que la referencia anterior se marcará
como inactiva. De esta forma, siempre tendremos un histórico de las referencias utilizadas en cada ensayo, siguiendo el id. La selección
del ensayo a trabajar se hará mediante la selección de la referencia activa, y se mostrarán los parámetros de ensayo asociados a dicha referencia.
También se permitirá ver las anteriores y seleccionar cualquiera de ellas para trabajar con sus parámetros de ensayo asociados.
		- referencia (VARCHAR(255), UNIQUE) - Nombre o código de la referencia.
		- descripcion (TEXT) - Descripción detallada de la referencia.
		- fecha_creacion (DATETIME) - Fecha y hora de creación de la referencia.
		- fecha_modificacion (DATETIME) - Fecha y hora de la última modificación de la referencia.
		- imagen (BLOB) - Imagen de la referencia, para mostrar en la aplicación durante el ensayo.
		
	- Tabla "ParametrosEnsayo":
		- id (INT, PRIMARY KEY, AUTO_INCREMENT)
		- referencia_id (INT) - Clave foránea que relaciona con la tabla Referencias.
		- nombre_contacto (VARCHAR(20)) - Nombre del contacto a ensayar
		- nPasoEnsayo (INT) - Número de paso del ensayo, para definir la secuencia de pasos a realizar en el ensayo.
		- nSalida (boolarray) - Array de 48 bool para indicar las salidas a activar en el ensayo. 
		- resistenciaNominal (float) - Valor de resistencia aceptado como bueno (Centro).
		- tolerancia (float) - Tolerancia aceptada para el ensayo, por ejemplo, un ohm de la resistencia nominal.
		- offset (float) - Valor de offset a restar a la resistencia medida, para compensar la caída óhmica adicional.
		- fecha_creacion (DATETIME) - Fecha y hora de creación del parámetro de ensayo.
		- fecha_modificacion (DATETIME) - Fecha y hora de la última modificación del parámetro de ensayo.
		- posX (INT) - Posición X del punto (dot) en la imagen para mostrar gráficamente la posición del contacto a ensayar.
		- posY (INT) - Posición Y del punto (dot) en la imagen para mostrar gráficamente la posición del contacto a ensayar.

### Resultados
		La tabla de resultados almacenará los resultados de cada ensayo, registrando al operario que los ha realizado,
el idReferenica (Para poder consultar los parámetros) el lote, la fecha y hora de realización del ensayo, el resultado total del ensayo
(Bueno/Malo). En una segunda tabla auxiliar, se almacenarán los resultados detallados de cada ensayo, como por ejemplo la resistencia
medida en cada paso del ensayo, la evaluación OK,NOK, y el valor analógico RAW.. 
	- Tabla "Resultados":
		- id (INT, PRIMARY KEY, AUTO_INCREMENT)
		- referencia_id (INT) - Clave foránea que relaciona con la tabla Referencias. Esta nos da trazabilidad de los parámetros con
los que se ha realizado el ensayo, ya que cada referencia tiene asociados unos parámetros de ensayo, y cada resultado se asocia a una referencia,
por lo que siempre podremos saber con qué parámetros se ha realizado cada ensayo, garantizando la trazabilidad de los ensayos realizados.
		- fecha_prueba (DATETIME) - Fecha y hora de realización de la prueba.
		- resultado (BOOLEAN) - Resultado total del ensayo.
	- Tabla "ResultadosDetalle":
		- id (INT, PRIMARY KEY, AUTO_INCREMENT)
		- resultado_id (INT) - Clave foránea que relaciona con la tabla Resultados, para saber a qué ensayo corresponde cada resultado detallado.
		- resistencia_medida (FLOAT) - Valor de resistencia medido durante el ensayo.


- PRUEBA DE GIT