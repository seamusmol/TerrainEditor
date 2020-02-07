Note:
In order to start editing terrain, click on the file(+) icon and then click on the file(+) icon under the WorldFile Tab.<br/>
Add Additional Chunks: Click on WordFile Tab then click on the desired region and click on the file(+) icon under the WordFile Tab.<br/>
Most Values are controlled using scrollable icon.<br/>
Scroll over the icon to change the value. <br/>
Holding left control changes it by 5.<br/>
It is recommeneded to lightly apply brush to the terrain to avoid stuttering/misclicking.<br/>
<br/>
<br/>
Terrain Material:<br/>
 The first icon applies the main material to terrain.<br/>
 The second icon applies a secondary material to the terrain using linear interpolation. <br/>
   The blending values is determined by the brush with the pencil and A icon.<br/>
 The third icon applies a decay to the terrain using overlay blending.<br/>
   The overlay blending value is determined by the brush with the pencil and A icon.<br/>
<br/>
Water:<br/>
 This tab controls physical aspects of water.<br/>
 The top icon enables Water.<br/>
 The second and third icons allow leveling and adjustment of water height.<br/>
 The fourth and fifth icon control water displacement distance. <br/>
   This requires a displacement height value greater than 0 to be noticeable.<br/>
 The sixth and seventh icon control water displacement height. <br/>
   This requires a water displacement distance greater than 0 to be noticeable.<br/>
<br/>
Water Material:<br/>
 <br/>
 Blue Wave Icon:<br/>
  The first icon controls water flow direction. Use the compass in the bottom left to control direction. <br/>
  The second icon merges different water flow directions using the four corners of the screen highlight square and flow Value.<br/>
  The third icon sets the time offset for water flowback.<br/>
   *This is only noticeable when the flowback value of water is greater than zero.<br/>
  The fourth icon sets the flowback speed of water.<br/>
   *This feature is useful for waves bouncing off the shorelines. <br/>
  The Fifth icon sets the normal insensity of water.<br/>
  The Sixth icon smoothens water normal values using the four corners of the screen highlight square and flow Value.<br/>
    <br/>
  
 White Wave Icon: <br/>
  The first icon applies foam to the water using the A icon value.<br/>
  The second icon smoothens foam using the four corners of the green highlight square and flow Value.<br/>
   <br/>
 Pencil Icon:<br/>
   The first icon sets the water color using the color slider in the bottom left corner.
   The second icon merges the water color using the four corners of the screen highlight square and flow Value.
   The third icon sets the reflectivity of the water. Normal water reflectivity has a refractive index of 1.33 or roughly A = 128.
   The fourth icon blends water refractive values using the four corners of the screen highlight square and flow Value.
   The fifth icon sets the amount that water color opacity is affected by water depth. 
   The Sixth icon blends the water depth opacity value using the four corners of the screen highlight square and flow Value.
 <br/>
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

