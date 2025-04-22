using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleController : MonoBehaviour
{
    [SerializeField] private PaddleSO paddleSO;
    [SerializeField] private ScreenEdgesSO screenEdgesSO;
    [SerializeField] private Transform visual;
    
    private Vector3 initialPosition;
    private float originalWidth;
    private Coroutine widePaddleCoroutine;
    private bool isWidePaddle = false;

    public void Initiate()
    {
        PaddlePhysics.Initiate(transform, visual, paddleSO, screenEdgesSO);
        initialPosition = transform.position;
        originalWidth = paddleSO.width;

        EventManager.OnReset += ResetPaddle;
    }

    private void ResetPaddle()
    {
        transform.position = initialPosition;
        
        if (isWidePaddle)
        {
            StopWidePaddlePowerUp();
        }
    }
    
    public void ActivateWidePaddle(float widthMultiplier = 1.5f, float duration = 5f)
    {
        if (widePaddleCoroutine != null)
        {
            StopCoroutine(widePaddleCoroutine);
        }
        
        widePaddleCoroutine = StartCoroutine(WidePaddlePowerUpRoutine(widthMultiplier, duration));
    }
    
    public void StopWidePaddlePowerUp()
    {
        if (widePaddleCoroutine != null)
        {
            StopCoroutine(widePaddleCoroutine);
            widePaddleCoroutine = null;
        }
        
        paddleSO.width = originalWidth;
        PaddlePhysics.UpdateWidth(paddleSO.width);
        isWidePaddle = false;
        
        Debug.Log("Paddle vuelve a su tamaño normal");
    }
    
    private IEnumerator WidePaddlePowerUpRoutine(float widthMultiplier, float duration)
    {
        if (originalWidth <= 0)
        {
            originalWidth = paddleSO.width;
        }
        
        float newWidth = originalWidth * widthMultiplier;
        paddleSO.width = newWidth;
        PaddlePhysics.UpdateWidth(newWidth);
        isWidePaddle = true;
        
        Debug.Log($"Paddle ampliado a {newWidth} durante {duration} segundos");
        
        yield return new WaitForSeconds(duration);
        
        paddleSO.width = originalWidth;
        PaddlePhysics.UpdateWidth(originalWidth);
        isWidePaddle = false;
        widePaddleCoroutine = null;
        
        Debug.Log("Paddle vuelve a su tamaño normal");
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(PaddlePhysics.bounds.center, PaddlePhysics.bounds.size);
        
        if (visual != null)
        {
            Gizmos.color = Color.red;
            Bounds meshBounds = visual.GetComponent<MeshFilter>()?.sharedMesh?.bounds ?? new Bounds(Vector3.zero, Vector3.one);
            Vector3 size = Vector3.Scale(meshBounds.size, visual.lossyScale);
            Vector3 center = visual.position + Vector3.Scale(meshBounds.center, visual.lossyScale);
            Gizmos.DrawWireCube(center, size);
        }
    }
#endif
}