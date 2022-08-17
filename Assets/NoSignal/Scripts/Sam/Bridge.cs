using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bridge : MonoBehaviour
{
    [Header("Gameplay Settings")]
    [SerializeField] private bool activateOnStart;
    [SerializeField] private bool lockAfterActivation;

    [Header("Visual Settings")]
    [SerializeField] float animationSpeed;
    [SerializeField] float activeAlpha;
    [SerializeField] float bridgeWidth;
    [SerializeField] float bridgeThickness;
    [SerializeField] float bridgeEndLength;
    [SerializeField] Vector2 bridgeEndMargin;
    [SerializeField] Vector2 bridgeColliderMargin;

    [Header("General Settings")]
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Collider bridgeCollider;
    [SerializeField] Transform pointA;
    [SerializeField] Transform pointB;
    [SerializeField] Transform bridgeCenterVisual;
    [SerializeField] Transform bridgeCenterCollider;

    private Tween bridgeAnimation;
    private bool isLocked;

    private void Start()
    {
        InitializeBridge();
    }

    void InitializeBridge()
    {
        pointA.LookAt(pointB);
        pointB.LookAt(pointA);
        Vector3 centerPosition = Vector3.Lerp(pointA.position, pointB.position, 0.5f);
        bridgeCenterVisual.position = centerPosition;
        bridgeCenterCollider.position = centerPosition;
        bridgeCenterVisual.LookAt(pointB);
        bridgeCenterCollider.LookAt(pointB);

        float distance = Vector3.Distance(pointA.position, pointB.position);
        Vector3 centerScale = new Vector3(bridgeWidth, bridgeThickness, distance);
        Vector3 endScale = centerScale + (Vector3)bridgeEndMargin;
        endScale.z = bridgeEndLength;

        bridgeCenterVisual.localScale = centerScale;
        bridgeCenterCollider.localScale = centerScale + (Vector3)bridgeColliderMargin;
        pointA.localScale = endScale;
        pointB.localScale = endScale;

        if (activateOnStart) ActivateBridge(true); else DeactivateBridge(true);
    }

    public void ActivateBridge()
    {
        ActivateBridge(false);
    }

    public void DeactivateBridge()
    {
        DeactivateBridge(false);
    }

    void ActivateBridge(bool completeInstantly)
    {
        if (isLocked) return;
        if (bridgeAnimation != null) bridgeAnimation.Kill();
        bridgeAnimation = meshRenderer.material.DOFade(activeAlpha, animationSpeed).SetSpeedBased(true).OnComplete(ActivateTransform);
        if (completeInstantly) bridgeAnimation.Complete();
        if (lockAfterActivation) isLocked = true;
    }

    void DeactivateBridge(bool completeInstantly)
    {
        if (isLocked) return;
        if (bridgeAnimation != null) bridgeAnimation.Kill();
        bridgeAnimation = meshRenderer.material.DOFade(0f, animationSpeed).SetSpeedBased(true).OnComplete(DeactivateTransform);
        if (completeInstantly) bridgeAnimation.Complete();
    }

    void ActivateTransform()
    {
        bridgeCollider.enabled = true;
    }

    void DeactivateTransform()
    {
        bridgeCollider.enabled = false;
    }
}
