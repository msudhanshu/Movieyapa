var howLong = 1.0;
var howFast = 8.0;
private var nextUpdate = 0.0;
private var direction : Vector3;

function Update () {
    if (Time.time > nextUpdate) {
        nextUpdate = Time.time + (Random.value * howLong);
        direction = Random.onUnitSphere;
        direction.y = 0;
        direction.Normalize ();
        direction *= howFast;
        direction.y = 1.5 - transform.position.y;
    }
    var controller = GetComponent(CharacterController);
    controller.Move(direction * Time.deltaTime);
}
