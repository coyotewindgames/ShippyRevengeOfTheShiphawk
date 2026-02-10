# Laser Machine Setup Instructions

## 1. Create a LaserData Scriptable Object
- In the **Project** window, right‑click.
- Select **Create → LaserMachine → LaserData**.
- A new LaserData asset will be created.

## 2. Fill in the LaserData Fields

### Material
- **Required.**
- If left empty, Unity will assign the magenta “missing material” to your lasers.

### Sparks
- Assign a prefab to use as the spark effect when the laser hits something.

### Properties
- These properties match the ones in the **LaserMachine** component.
- Both use the same underlying C# class.

## 3. Create the Laser Machine GameObject
- Create an empty GameObject (this will act as the **center point**).
- Add the **LaserMachine** component.

### Orientation Notes
- The initial laser fires along the **local forward** direction.
- Additional lasers rotate around the **local up** axis.

## 4. Assign the LaserData Asset
- Drag your LaserData ScriptableObject into the LaserMachine component.

## 5. Configure Properties

### Using ScriptableObject Properties (Default)
- All LaserMachine instances using this LaserData will share these settings.

### Override Per‑Instance Properties
- Enable **Override External Properties** in the LaserMachine component.
- This allows custom settings for that specific GameObject.
