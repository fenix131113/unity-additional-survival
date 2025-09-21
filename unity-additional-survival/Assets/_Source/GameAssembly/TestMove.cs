using System.Collections;
using UnityEngine;

public class TestMove : MonoBehaviour
{
    [SerializeField] private float speed;
    private Rigidbody2D _rb;
    private bool _moveRight = true;
        
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        StartCoroutine(Sc());
    }
    
    private void FixedUpdate()
    {
        var v = _moveRight ? Vector2.right : Vector2.left;
        var moveVector = (Vector2)transform.position + v * (speed * Time.fixedDeltaTime);
        _rb.MovePosition(moveVector);
    }

    private IEnumerator Sc()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            
            _moveRight = !_moveRight;
        }
    }
}