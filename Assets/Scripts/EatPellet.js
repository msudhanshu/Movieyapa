var scoreDisplay : GUIText;
var smallPelletScore = 10;
var superPelletScore = 100;
private var score = 0;

function Update () {
    scoreDisplay.text = "Score: " + score;
}

function OnTriggerEnter (other : Collider) {
    if (other.name == "BasicPellet(Clone)") {
        score += smallPelletScore;
    } else if (other.name == "SuperPellet(Clone)") {
        score += superPelletScore;
    }
    Destroy (other.gameObject);
}
