# PC7866 - Arquitectura y Documentación Técnica

## 📐 Estructura del Proyecto

```
PC7866/
├── Models/                          ✅ CREADO
│   ├── TestParameters.cs            # Parámetros de configuración
│   ├── TestResult.cs                # Resultados de tests
│   ├── MeasurementCommand.cs        # Comandos individuales
│   ├── MeasurementResult.cs         # Resultados de mediciones
│   └── DeviceResponse.cs            # Respuestas del dispositivo
│
├── Services/                        ✅ CREADO
│   └── SerialCommunication/
│       ├── ISerialPortService.cs    # Interfaz de comunicación
│       ├── SerialPortService.cs     # Implementación puerto serie
│       └── CommandParser.cs         # Parser de comandos
│
├── Views/                           ✅ CREADO
│   ├── ManualControlForm.cs         # Formulario modo manual
│   └── ManualControlForm.Designer.cs
│
├── Utils/                           ✅ CREADO
│   └── Logger.cs                    # Sistema de logging
│
├── Configuration/                   ✅ CREADO
│   └── AppSettings.cs               # Configuración global
│
└── [PENDIENTE]
    ├── Services/Database/           # Acceso a MariaDB (próximo paso)
    ├── Services/StateMachine/       # Máquina de estados (próximo paso)
    └── Views/AutomaticTestForm.cs   # Modo automático (próximo paso)
```

---

## 🔧 Componentes Implementados

### 1️⃣ **SerialPortService** - Comunicación Serie USB

**Características:**
- ✅ Comunicación asíncrona (async/await)
- ✅ Manejo de timeouts
- ✅ Eventos para datos recibidos y errores
- ✅ Thread-safe (usa SemaphoreSlim)
- ✅ Buffer de recepción inteligente
- ✅ Configuración flexible (baudrate, paridad, bits)

**Ejemplo de uso:**
```csharp
var serialPort = new SerialPortService();

// Conectar
serialPort.Open("COM3", 9600);

// Enviar comando y esperar respuesta
string response = await serialPort.SendCommandAsync("*IDN?", 5000);

// Cerrar
serialPort.Close();
```

**Eventos disponibles:**
- `DataReceived` - Datos recibidos del dispositivo
- `ErrorOccurred` - Error en comunicación
- `PortOpened` - Puerto abierto exitosamente
- `PortClosed` - Puerto cerrado

---

### 2️⃣ **CommandParser** - Parser de Comandos

**Funciones:**
- Parsea respuestas del dispositivo
- Extrae valores numéricos
- Valida respuestas contra patrones esperados
- Detecta códigos de error

**Ejemplo:**
```csharp
var parser = new CommandParser();

// Parsear respuesta
var response = parser.ParseResponse("RESISTANCE=1234.56");

// Extraer valor numérico
decimal? value = parser.ExtractNumericValue(response.RawData);

// Validar patrón
bool valid = parser.ValidateResponse(response.RawData, @"RESISTANCE=\d+");
```

---

### 3️⃣ **Modelos de Datos**

#### TestParameters
Define la configuración de un test:
- Secuencia de comandos
- Timeouts
- Tolerancias
- Metadata

#### MeasurementCommand
Comando individual con:
- Comando a enviar
- Patrón de respuesta esperado
- Delay después del comando
- Criticidad (¿fallar test si falla?)

#### TestResult
Resultado completo:
- Estado (Passed/Failed/Error)
- Lista de mediciones individuales
- Duración
- Observaciones

---

### 4️⃣ **ManualControlForm** - Interfaz Modo Manual

**Funcionalidades:**
- ✅ Selección de puerto y velocidad
- ✅ Conexión/desconexión
- ✅ Envío de comandos con Enter o botón
- ✅ Log en tiempo real con timestamps
- ✅ Manejo de errores y timeouts
- ✅ Indicador de estado en barra inferior

**Controles:**
- ComboBox para puertos disponibles
- ComboBox para velocidades (9600-115200)
- TextBox para comandos
- Log estilo terminal (fondo negro, texto verde)
- Botones: Conectar, Desconectar, Enviar, Limpiar

---

### 5️⃣ **Logger** - Sistema de Logging

**Características:**
- Singleton pattern
- Logs guardados en: `%LocalAppData%\PC7866\Logs\`
- Archivo diario: `log_YYYYMMDD.txt`
- Niveles: Debug, Info, Warning, Error
- También escribe en Debug output

**Uso:**
```csharp
Logger.Instance.Info("Conexión establecida");
Logger.Instance.Error($"Error: {ex.Message}");
```

---

### 6️⃣ **AppSettings** - Configuración Global

**Parámetros configurables:**
```csharp
// Puerto serie
DefaultPortName = "COM1"
DefaultBaudRate = 9600
DefaultTimeout = 5000

// Base de datos
DatabaseServer = "localhost"
DatabaseName = "pc7866_test"
DatabaseUser = "root"
DatabasePort = 3306

// Test
MaxRetries = 3
DelayBetweenCommandsMs = 100
AutoSaveResults = true
```

---

## 🚀 Cómo Probar el Modo Manual

1. **Compilar el proyecto:**
   ```bash
   dotnet build
   ```

2. **Ejecutar:**
   ```bash
   dotnet run
   ```

3. **Usar la interfaz:**
   - Seleccionar puerto COM
   - Seleccionar velocidad (ej: 9600)
   - Clic en "Conectar"
   - Escribir comando (ej: `*IDN?`)
   - Presionar Enter o "Enviar"
   - Ver respuesta en el log

---

## 📦 Dependencias NuGet

```xml
<PackageReference Include="System.IO.Ports" Version="8.0.0" />
```

**Para próximas fases:**
```xml
<!-- Base de datos -->
<PackageReference Include="MySqlConnector" Version="2.*" />
<PackageReference Include="Dapper" Version="2.*" />

<!-- Exportación Excel -->
<PackageReference Include="ClosedXML" Version="0.102.*" />
```

---

## 🎯 Próximos Pasos

### Fase 3: Base de Datos MariaDB
- [ ] Crear esquema de base de datos
- [ ] Implementar `MariaDbContext`
- [ ] Implementar repositorios (TestRepository, ParametersRepository)
- [ ] Migraciones

### Fase 4: Máquina de Estados
- [ ] Diseñar estados del test
- [ ] Implementar `TestStateMachine`
- [ ] Implementar estados individuales (Idle, Running, Measuring, etc.)
- [ ] Integrar con SerialPortService

### Fase 5: Modo Automático
- [ ] Crear `AutomaticTestForm`
- [ ] Cargar secuencias desde BD
- [ ] Ejecutar tests automáticos
- [ ] Guardar resultados en BD
- [ ] Exportar a CSV/Excel

---

## 🔍 Patrones de Diseño Utilizados

1. **Singleton**: `Logger`, `AppSettings`
2. **Repository Pattern**: (pendiente para BD)
3. **State Machine**: (pendiente)
4. **Dependency Injection**: Uso de interfaces (`ISerialPortService`)
5. **Event-Driven**: Eventos para comunicación asíncrona
6. **Async/Await**: Todas las operaciones I/O son asíncronas

---

## 📝 Convenciones de Código

- ✅ Nullable reference types habilitado
- ✅ Namespaces file-scoped
- ✅ Async suffix en métodos asíncronos
- ✅ Interfaces con prefijo `I`
- ✅ Campos privados con `_` prefix
- ✅ Comentarios XML en APIs públicas

---

## 🐛 Debugging

**Logs se guardan en:**
```
%LocalAppData%\PC7866\Logs\log_YYYYMMDD.txt
```

**Para ver logs en tiempo real:**
- Visual Studio: Output window (Debug)
- Interfaz: Log de comunicación

---

## 📚 Referencias

- [System.IO.Ports Documentation](https://docs.microsoft.com/dotnet/api/system.io.ports)
- [Async/Await Best Practices](https://docs.microsoft.com/archive/msdn-magazine/2013/march/async-await-best-practices-in-asynchronous-programming)
- [Windows Forms .NET](https://docs.microsoft.com/dotnet/desktop/winforms/)
