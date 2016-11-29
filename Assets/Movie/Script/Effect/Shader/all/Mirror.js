
@script ExecuteInEditMode()
 
var objectBeforeMirror : GameObject;
var mirrorPlane : GameObject;
 
function Update () 
{
   if (null != mirrorPlane) 
   {
      GetComponent.<Renderer>().sharedMaterial.SetMatrix("_WorldToMirror", 
         mirrorPlane.GetComponent.<Renderer>().worldToLocalMatrix);
      if (null != objectBeforeMirror) 
      {
         transform.position = objectBeforeMirror.transform.position;
         transform.rotation = objectBeforeMirror.transform.rotation;
         transform.localScale = 
            -objectBeforeMirror.transform.localScale; 
         transform.RotateAround(objectBeforeMirror.transform.position, 
            mirrorPlane.transform.TransformDirection(
            Vector3(0.0, 1.0, 0.0)), 180.0);
 
         var positionInMirrorSpace : Vector3 = 
            mirrorPlane.transform.InverseTransformPoint(
            objectBeforeMirror.transform.position);
         positionInMirrorSpace.y = -positionInMirrorSpace.y;
         transform.position = mirrorPlane.transform.TransformPoint(
            positionInMirrorSpace);
      }
   }
}