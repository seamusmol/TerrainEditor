Note:<br/>
Run Visual Studio as Adminstrator before opening project.<br/>
In order to start editing terrain, click on the file(+) icon and then click on the file(+) icon under the WorldFile Tab.<br/>
Add Additional Chunks: Click on WordFile Tab then click on the desired region and click on the file(+) icon under the WordFile Tab.<br/>
Most Values are controlled using scrollable icon.<br/>
Scroll over the icon to change the value. <br/>
Holding left control changes it by 5.<br/>
It is recommeneded to lightly apply brush to the terrain to avoid stuttering/misclicking.<br/>
<br/>
<br/>
Terrain Material:
 <ul>
 <li>The first icon applies the main material to terrain.</li>
 <li>The second icon applies a secondary material to the terrain using linear interpolation.</li>
   The blending values is determined by the brush with the pencil and A icon.
 <li>The third icon applies a decay to the terrain using overlay blending.</li>
   The overlay blending value is determined by the brush with the pencil and A icon.</li>
</ul>
Water:<br/>
<ul>
 <li>This tab controls physical aspects of water.</li>
 <li>The top icon enables Water.</li>
 <li>The second and third icons allow leveling and adjustment of water height.</li>
 <li>The fourth and fifth icon control water displacement distance. </li>
  <ul><li>This requires a displacement height value greater than 0 to be noticeable.</li></ul>
 <li>The sixth and seventh icon control water displacement height.</li></ul>
  <ul><li>This requires a water displacement distance greater than 0 to be noticeable.</li></ul>
</ul>
Water Material:<br/>
Blue Wave Icon:<br/>
 <ul>
  <li>The first icon controls water flow direction. Use the compass in the bottom left to control direction. </li>
  <li>The second icon merges different water flow directions using the four corners of the screen highlight square and flow Value.</li>
  <li>The third icon sets the time offset for water flowback.</li>
  *This is only noticeable when the flowback value of water is greater than zero.
  <li>The fourth icon sets the flowback speed of water.</li>
  *This feature is useful for waves bouncing off the shorelines.
  <li>The Fifth icon sets the normal insensity of water.</li>
  <li>The Sixth icon smoothens water normal values using the four corners of the screen highlight square and flow Value.</li>
 </ul>
  
 White Wave Icon: <br/>
 <ul>
  <li>The first icon applies foam to the water using the A icon value.</li>
  <li>The second icon smoothens foam using the four corners of the green highlight square and flow Value.</li>
 </ul> 
 <br/>
 Pencil Icon:
 <ul>
   <li>The first icon sets the water color using the color slider in the bottom left corner.</li>
    <li>The second icon merges the water color using the four corners of the screen highlight square and flow Value.</li>
    <li>The third icon sets the reflectivity of the water. Normal water reflectivity has a refractive index of 1.33 or roughly A = 128.</li>
    <li>The fourth icon blends water refractive values using the four corners of the screen highlight square and flow Value.</li>
    <li>The fifth icon sets the amount that water color opacity is affected by water depth </li>
    <li>The Sixth icon blends the water depth opacity value using the four corners of the screen highlight square and flow Value.</li>
 </ul>
Objects:
<ul>
 <li>Models can be added by importing obj models into the Models folder.</li>
 <li>Each Model requires a Model material file. This file specifies the shaders and PBR textures for the object.</li>
 <li>Model vertices are not normalized. Models can be scaled,rotated and translated globally.</li>
</uL>
Controls: <br/>
<ul>
 <li>Zoom In/Out: Mouse Scroll </li>
 <li>Rotate Camera Left: Q </li>
 <li>Rotate Camera Right: E </li>
 <li>Rotate Camera: Hold Middle Mouse + Move Mouse</li>
 <li>Camera Movement: WASD </li>
 <li>Camera Movement Up: Left Shift </li>
 <li>Camera Movement Down: Left Control </li>
</ul>
Prop Controls: <br/>
<ul>
 <li>Move Object: T </li>
 <li>Set Movement Axis: X,Y,Z </li>
 <li>Select Object: Right Mouse </li>
 <li>Deselect Object: Right Mouse(not on object) </li>
 <li>Cancel Object Movement: Right Mouse Click(when selected) </li>
</ul>
