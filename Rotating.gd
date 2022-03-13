extends CSGSphere


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	transform = transform.rotated(Vector3.UP, -0.5 * delta)
