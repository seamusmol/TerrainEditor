Note:
In order to start editing terrain, click on the file(+) icon and then click on the file(+) icon under the WorldFile Tab.
Add Additional Chunks: Click on WordFile Tab then click on the desired region and click on the file(+) icon under the WordFile Tab
Most Values are controlled using scrollable icon. Scroll over the icon to change the value. Holding left control changes it by 5.
It is recommeneded to lightly apply brush to the terrain to avoid stuttering/misclicking.


Terrain Material:
 The first icon applies the main material to terrain.
 The second icon applies a secondary material to the terrain using linear interpolation. 
   *The blending values is determined by the brush with the pencil and A icon.
 The third icon applies a decay to the terrain using overlay blending.
   *The overlay blending value is determined by the brush with the pencil and A icon.

Water:
 This tab controls physical aspects of water.
 The top icon enables Water.
 The second and third icons allow leveling and adjustment of water height.
 The fourth and fifth icon control water displacement distance. 
   *This requires a displacement height value greater than 0 to be noticeable.
 The sixth and seventh icon control water displacement height. 
   *This requires a water displacement distance greater than 0 to be noticeable.
  
Water Material:
 *Blue Wave Icon:
  The first icon controls water flow direction. Use the compass in the bottom left to control direction. 
  The second icon merges different water flow directions using the four corners of the screen highlight square and flow Value.
  The third icon sets the time offset for water flowback.
   *This is only noticeable when the flowback value of water is greater than zero.
  The fourth icon sets the flowback speed of water.
   *This feature is useful for waves bouncing off the shorelines. 
  The Fifth icon sets the normal insensity of water.
  The Sixth icon smoothens water normal values using the four corners of the screen highlight square and flow Value.
   
  
 *White Wave Icon:
  The first icon applies foam to the water using the A icon value.
  The second icon smoothens foam using the four corners of the green highlight square and flow Value.
  
 *Pencil Icon:
   The first icon sets the water color using the color slider in the bottom left corner.
   The second icon merges the water color using the four corners of the screen highlight square and flow Value.
   The third icon sets the reflectivity of the water. Normal water reflectivity has a refractive index of 1.33 or roughly A = 128.
   The fourth icon blends water refractive values using the four corners of the screen highlight square and flow Value.
   The fifth icon sets the amount that water color opacity is affected by water depth. 
   The Sixth icon blends the water depth opacity value using the four corners of the screen highlight square and flow Value.
 
Objects:
 Models can be added by importing obj models into the Models folder.
 Each Model requires a Model material file. This file specifies the shaders and PBR textures for the object.
 Model vertices are not normalized. Models can be scaled,rotated and translated globally.

Controls:
 Zoom In/Out: Mouse Scroll
 Rotate Camera Left: Q
 Rotate Camera Right: E
 Rotate Camera: Hold Middle Mouse + Move Mouse

 Camera Movement: WASD
 Camera Movement Up: Left Shift
 Camera Movement Down: Left Control

Prop Controls:
 Move Object: T
 Set Movement Axis: X,Y,Z
 Select Object: Right Mouse
 Deselect Object: Right Mouse(not on object)
 Cancel Object Movement: Right Mouse Click(when selected)

