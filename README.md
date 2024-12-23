# AimAssist_Unity2D

**Aim Assist** is a Unity script designed to provide smooth and dynamic aim correction in 2D top-down games. The script enhances player experience by assisting their aim subtly without using harsh "snapping." It is a rewritten and forked version of Mr. Giff's GML code, adapted for Unity 2D by **ugurevren**.  

This implementation is inspired by @t3ssel8r's research and video on aim assist systems, which can be found [here](https://www.youtube.com/watch?v=yGci-Lb87zs).  

## Features  
- **Adjustable Aim Tolerance:** Control how precise the player’s aim needs to be before correction is applied.  
- **Smoothing Options:** Configure aim assist behavior from snappy to smooth corrections.  
- **Dynamic Target Detection:** Automatically detects and tracks visible enemies in real time.  
- **Flexible Settings:** Easily customize parameters such as correction range, maximum angle, and smoothing through the Unity Inspector.  

## Dependencies  
- Unity Camera System  

## How to Use  
1. Attach the `AimAssist` script to a GameObject, such as the player.  
2. Assign the list of enemies (`Transform[] _enemies`) via the Unity Inspector.  
3. Configure parameters like `_maxCorrection`, `_maxAngle`, and `_smoothing` in the Inspector to suit your game.  
4. Use the `AssistedAim` method in your player controller script to apply aim correction dynamically.  

### Example Usage
- **AimAssist.cs** also includes an example usage that you can check out.
  
```csharp  
// Example: Applying aim correction in the player's script  
float playerAngle = Mathf.Atan2(aimDirection.y, aimDirection.x);  
float correctedAngle = aimAssist.AssistedAim(playerAngle, playerTransform);  

// Update player aim direction based on corrected angle  
Vector3 correctedDirection = new Vector3(Mathf.Cos(correctedAngle), Mathf.Sin(correctedAngle), 0);  
playerTransform.up = correctedDirection;
  
