# Handoff — Magic Game Jamming

## Propósito de este documento

Este archivo permite que otra persona o agente continúe el proyecto Unity sin reconstruir el contexto desde cero. Describe el diseño acordado, el estado técnico **verificado en los archivos reales** y los próximos pasos. No es una especificación final de arte ni de balance: varias decisiones visuales siguen abiertas a propósito.

## Estado verificado

- Proyecto Unity 6.4.6f1 (`6000.4.6f1`).
- Proyecto 2D, resolución de trabajo: **1920 × 1080 (Full HD)**.
- El proyecto usa el Input System nuevo; el drag usa `UnityEngine.InputSystem.Mouse.current`.
- Git estaba limpio al momento de redactar este handoff. Último commit: `977277b Juego bastante avanzado`.
- Escenas incluidas en la lista de compilación:
  - `Assets/Scenes/Scn4 - Game/ScnGame.unity`
  - `Assets/Scenes/Scn5 - PostGame/Scn5 - PostGame.unity`
- `Scn1 - Menu` y `Scn2 - Intro` existen como carpetas, pero todavía no hay escenas `.unity` implementadas en ellas.

## Concepto del juego

Juego 2D de alquimia con drag & drop. El jugador controla una bruja que mezcla ingredientes en un caldero para satisfacer pedidos de clientes.

Cada cliente solicita una de cuatro pociones. El jugador agrega ingredientes repetibles al caldero, ve los atributos acumulados y pulsa **Brew** cuando decide evaluar su mezcla. La mezcla puede ser perfecta, exitosa o fallida. El resultado determina oro y estadísticas de la jornada.

La jornada dura seis minutos en la versión objetivo. Cada cliente espera 30 segundos. Un cliente resuelto por Brew, o uno que se va por tiempo, deja una pausa de cinco segundos antes de que aparezca el siguiente pedido.

No existe ni se necesita botón `Reject`: una mezcla fallida o dejar ir al cliente representan la misma decisión de diseño.

## Reglas matemáticas

El orden de todos los atributos es siempre:

```text
Dulzura / Energía / Frescura / Intensidad
```

- Toda mezcla comienza en `0 / 0 / 0 / 0`.
- Los ingredientes se pueden usar repetidamente; no hay límite de ingredientes implementado.
- Una poción es **Perfecta** si los cuatro atributos coinciden exactamente con el target.
- Es **Exitosa** si cada atributo individual está a distancia máxima de `±1` del target.
- Es **Fallida** si algún atributo se desvía más de `±1`.
- La tolerancia es por atributo, no por promedio ni por suma de distancias.

### Ingredientes actuales

| Ingrediente | Dulzura | Energía | Frescura | Intensidad |
|---|---:|---:|---:|---:|
| Luces | +1 | +2 | +1 | 0 |
| PaloSanto | +1 | -2 | -1 | +1 |
| Polvo | +1 | 0 | 0 | +2 |
| Flores | +1 | +1 | 0 | -1 |
| Cuernos | -2 | -2 | 0 | +1 |
| Menta | 0 | 0 | +1 | +2 |

La reasignación de nombres/arte a estos modificadores fue intencional y conserva la matriz matemática original: cambió qué sprite representa cada vector, no el espacio de soluciones.

### Pócimas actuales

Los assets viven en `Assets/Scenes/Scn4 - Game/Data/Pociones/`.

| Asset | Nombre actual | Target |
|---|---|---:|
| `Pot1.asset` | Pot1 | `2 / 2 / 2 / 2` |
| `Pot2.asset` | Pot2 | `5 / 5 / 5 / 5` |
| `Pot3.asset` | Pot3 | `3 / 2 / 4 / 6` |
| `Pot4.asset` | Pot4 | `7 / 1 / 0 / 5` |

Los nombres son temporales a propósito; pueden cambiarse sin romper la lógica porque los scripts usan referencias a los assets, no strings de nombre.

### Recompensas actuales

Para todas las pócimas:

```text
Perfecta: 10 oro
Exitosa:   5 oro
Fallida:   0 oro
```

Los valores se declaran por defecto en `PotionData.cs` como `goldPerfecta = 10` y `goldExitosa = 5`. En los assets existentes pueden no aparecer serializados hasta que Unity los guarde de nuevo, pero toman esos defaults en tiempo de ejecución.

## Escenas y jerarquía

### Scn4 — Game

Escena: `Assets/Scenes/Scn4 - Game/ScnGame.unity`.

Nodos principales:

```text
ScnGame
├── Main Camera
├── Background
├── Bruja
│   ├── Ingr-General
│   │   ├── Ingr (Base)  # ingredientes normales con colliders y scripts
│   │   ├── Ingr (HL)    # copias resaltadas, apagadas al inicio
│   │   └── Ingr (GH)    # ghosts, apagados al inicio
│   ├── Caldero
│   │   ├── CalderoObj / CalderoSprite
│   │   ├── Btn-Restart
│   │   └── Btn-Brew
│   └── Panel-Stats (PS)
│       └── textos de atributos actuales, targets y oro
├── Clientes
│   ├── ClienteObj
│   └── BurbujaPedido
└── GameManager
```

El arte visual actual es mayormente placeholder. Hay sprites de ingredientes y botones en `Assets/Scenes/Scn4 - Game/Visual/`. La intención es sustituir sprites sin alterar la lógica.

Orden de dibujo de referencia utilizado durante el prototipo:

```text
Background: 1
Normales / caldero / panel: 2
Highlights: 3
Botones: 4
Textos: 5
Ghosts: 8
```

Los clics se resuelven por colliders, no por `Order in Layer`; los botones deben permanecer fuera del collider de drop del caldero.

### Scn5 — PostGame

Escena: `Assets/Scenes/Scn5 - PostGame/Scn5 - PostGame.unity`.

Contiene `PostGamePanel` con textos fijos y referencias dinámicas para:

- oro recolectado;
- oro acumulado;
- pociones perfectas;
- pociones exitosas;
- pociones fallidas.

Por ahora, oro recolectado y oro acumulado muestran el mismo valor porque solo existe una jornada jugable. La diferencia queda preparada para futuras jornadas/niveles.

## Scripts actuales y responsabilidades

Todos los scripts de la escena principal viven en:

`Assets/Scenes/Scn4 - Game/Scripts/`

### `IngredientStats.cs`

Componente de cada ingrediente **normal**. Guarda nombre y los cuatro modificadores. Sus campos son privados serializados, y expone propiedades públicas de solo lectura para que otros scripts los consuman.

No se agrega a Highlight ni Ghost.

### `IngredientHover.cs`

Componente de cada ingrediente normal. Implementa:

- hover: enciende/apaga el Highlight correspondiente;
- inicio de drag: activa el Ghost;
- drag: mueve el Ghost con `Mouse.current.position`;
- drop: usa `Collider2D.OverlapPoint` contra el collider del caldero;
- si el drop es válido, llama `PotionMixer.AddIngredient(ingredientStats)`;
- al soltar, restaura el Ghost a su posición local inicial y lo apaga.

El ingrediente original no se mueve. El Ghost es la representación visual temporal del cursor.

### `PotionData.cs`

`ScriptableObject` con menú de creación `Pociones/Potion Data`. Guarda nombre, target de los cuatro atributos y recompensas de perfecta/existosa. Es la fuente de datos de cada pedido.

### `PotionMixer.cs`

Componente del nodo lógico `Caldero`, no del sprite/collider visual. Responsabilidades:

- acumular atributos actuales;
- actualizar TMPs de atributos actuales y targets;
- cargar una `PotionData` con `LoadPotion`;
- resetear la mezcla con `ResetMix`;
- evaluar con `Brew` y devolver el enum `BrewResult`:
  - `Perfecta`
  - `Exitosa`
  - `Fallida`

No gestiona clientes, oro ni cambio de escena.

### `RestartButton.cs`

Componente de `Btn-Restart`. En `OnMouseDown` llama a `PotionMixer.ResetMix()`.

### `BrewButton.cs`

Componente de `Btn-Brew`. En `OnMouseDown` llama a `CustomerManager.BrewCurrentPotion()`. El botón no decide pagos ni próximos clientes.

### `CustomerManager.cs`

Componente del nodo `Clientes`. Es el orquestador de pedidos dentro de una jornada.

- Elige una `PotionData` aleatoria de `availablePotions`.
- Evita repetir la misma poción en dos pedidos consecutivos.
- Carga el target en `PotionMixer`.
- Mantiene 30 segundos de espera por cliente (`customerWaitTime = 30`).
- Tras Brew o timeout, espera 5 segundos (`nextCustomerDelay = 5`) antes de resetear caldero y pedir otra poción.
- Un timeout registra una fallida.
- En Brew registra el resultado y entrega 10/5/0 oro según la `PotionData` actual.
- `EndDay()` detiene las coroutines y bloquea nuevos pedidos.

### `GameManager.cs`

Componente del nodo raíz `GameManager` de Scn4.

- Guarda oro y contadores de Perfecta/Exitosa/Fallida.
- Actualiza el TMP de oro de Scn4.
- Ejecuta el temporizador global de jornada.
- Al finalizar, detiene `CustomerManager` y carga `Scn5 - PostGame`.
- Usa `DontDestroyOnLoad(gameObject)` para sobrevivir la transición Scn4 → Scn5; de ese modo `PostGameDisplay` puede leer los datos.

**Estado guardado actual en la escena:** `dayDuration` está en `40` para pruebas. Volverlo a `360` para una jornada normal de seis minutos.

### `PostGameDisplay.cs`

Vive en `Assets/Scenes/Scn5 - PostGame/Scripts/` y está asignado a `PostGamePanel`.

En `Start`, encuentra el `GameManager` persistente y copia sus contadores a los cinco TMPs del panel final.

## Flujo funcional actual

```text
Inicio de Scn4
  → CustomerManager selecciona una poción aleatoria
  → PotionMixer actualiza los targets visibles
  → jugador arrastra ingredientes al caldero
  → mezcla visible se actualiza en tiempo real
  → Brew
      → CustomerManager recibe BrewResult
      → GameManager registra resultado y oro
      → pausa de 5 s
      → nuevo pedido no repetido inmediatamente
  → o timeout de 30 s
      → registra Fallida
      → pausa de 5 s
      → nuevo pedido
  → fin de jornada
      → GameManager detiene clientes
      → carga Scn5
      → PostGameDisplay muestra resumen
```

## Decisiones de diseño ya tomadas

- No mostrar countdown numérico: el tiempo del cliente debe sentirse intuitivo; se entiende cuando el primer cliente se va.
- El feedback de Brew no irá en un cartel junto al caldero.
- El feedback futuro será una reacción visual/sonora del cliente en la zona izquierda.
- La burbuja puede tener un sprite de fondo y, si se requiere texto variable, TMP encima. Alternativamente se pueden intercambiar sprites de burbujas ya compuestas.
- Se reutiliza **un solo** `ClienteObj`; se cambia skin/sprite y reacción para cada pedido. No hace falta instanciar nuevos objetos cliente.
- El botón `Reject` queda descartado como mecánica, aunque existe un asset antiguo `Reject Btn.png`.

## Trabajo pendiente, en orden recomendado

1. **Cerrar el ciclo de jornadas.**
   - Añadir a Scn5 botones `Continuar` y `Volver al menú`.
   - Refactorizar `GameManager` antes de reutilizar Scn4: hoy persiste con `DontDestroyOnLoad`, pero todavía no maneja una segunda jornada ni la posible duplicación de GameManagers al recargar Scn4.
   - Separar “oro de la jornada” de “oro acumulado” si se habilitan múltiples días.
   - Resetear correctamente contadores de día, timer y referencias de `CustomerManager` al empezar la siguiente jornada.

2. **Clientes y feedback.**
   - Agregar datos/skins de cliente y cambiar el `SpriteRenderer` al generar un pedido.
   - Mostrar reacción distinta para Perfecta/Exitosa/Fallida/timeout durante la pausa de 5 segundos.
   - Añadir sonidos de drop, Brew, éxito/fallo y salida de cliente.
   - Agregar animaciones de entrada/salida; por ahora el cambio ocurre lógicamente sin animación.

3. **Escenas iniciales.**
   - Crear y conectar `Scn1 - Menu` y `Scn2 - Intro` si se mantienen en el alcance.
   - Decidir qué botón inicia una jornada y cómo reinicia estado de sesión.

4. **Arte y UX.**
   - Reemplazar placeholders por arte final de fondo, caldero, paneles, clientes y botones.
   - Revisar colliders cuando cambien sprites, en especial el área de drop del caldero y los botones.
   - Ajustar tipografía, jerarquía visual y transiciones entre escenas.

5. **Balance futuro (no modificar sin simulación).**
   - La matriz de ingredientes actual es punto de partida.
   - Se quería simular cantidad de soluciones, mínimos de ingredientes, ingredientes dominantes y dificultad humana/virtual antes de cambiar la matriz o los targets.
   - Distinguir dificultad matemática, de optimización y humana.

## Riesgos / observaciones técnicas

- `GameManager` persiste entre Scn4 y Scn5. Eso funciona para el resumen actual, pero no está preparado para reiniciar/continuar una segunda jornada sin una refactorización de ciclo de vida.
- `GameManager.UpdateGoldUI()` no comprueba si `txtGold` es nulo. En Scn5 no vuelve a ejecutarse, por lo que el flujo actual funciona; al ampliar el ciclo de escenas, conviene robustecerlo.
- `PostGameDisplay` depende de que se entre a Scn5 desde Scn4. Si se ejecuta Scn5 directamente en el editor, no habrá `GameManager` persistente y los textos quedan con sus valores de editor.
- El juego hoy se prueba correctamente en el editor, pero este handoff no incluye un build final ni pruebas automatizadas.

## Prompt sugerido para el próximo agente

> Lee primero `HANDOFF.md`, luego inspecciona los scripts bajo `Assets/Scenes/Scn4 - Game/Scripts/` y `Assets/Scenes/Scn5 - PostGame/Scripts/`. No reemplaces la matriz de ingredientes ni los targets sin confirmar. El loop de una jornada funciona; el siguiente objetivo técnico es permitir Continue/Menu desde PostGame sin duplicar `GameManager` y preservando oro acumulado según la decisión de diseño.

