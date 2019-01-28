using UnityEngine;

public class FixedMovementPathing : MonoBehaviour {

    public float speed, range;
    public bool flipOnRotation;
    private Vector3 spawnLocation;
    private Vector3? lastLocation;

    private SpriteRenderer spriteRenderer;

    // FixedMovingPathing moves an object from a to b in a range. It can also flip the sprite if necessary.
    void Awake()
    {
        spawnLocation = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        transform.position = new Vector2(spawnLocation.x + range * Mathf.Sin(Time.time * speed), transform.position.y);

        if (flipOnRotation)
        {
            // If no last location, set.
            if (!lastLocation.HasValue)
            {
                lastLocation = transform.position;
                return;
            }

            // Whether the sprite is flipped is based on whether their last location x is greater than their current x.
            spriteRenderer.flipX = lastLocation.Value.x > transform.position.x;
            lastLocation = transform.position;
        }

    }

}
