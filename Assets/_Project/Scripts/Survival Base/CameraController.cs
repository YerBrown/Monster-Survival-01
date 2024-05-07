using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEngine.EventSystems.EventTrigger;

public class CameraController : MonoBehaviour
{
    private Vector3 _touchStart;
    private bool _IsZoomin = false;
    private Camera _mainCamera;
    private CinemachineVirtualCamera _cameraController;
    [SerializeField] private float _minZoom = 8f;
    [SerializeField] private float _maxZoom = 1f;

    public Vector2 CameraLimits;

    // Intento de new input system
    private SurvivalBaseInputs _Controlls;
    private bool _IsDragging;
    public bool WasDraggingInLastMovement;
    public float MinDistanceToDetectDrag = 0.2f;
    public bool DebugMode = false;
    private Coroutine _MovementCoroutine;
    private Coroutine _ZoomCoroutine;
    private void Awake()
    {
        _mainCamera = Camera.main;
        _cameraController = GetComponent<CinemachineVirtualCamera>();
        _Controlls = new SurvivalBaseInputs();
    }
    private void OnEnable()
    {
        _Controlls.Enable();
    }
    private void OnDisable()
    {
        _Controlls.Disable();
    }
    private void Start()
    {
        _Controlls.CameraMovement.FirstTouchContact.performed += _ => StartMovement();
        _Controlls.CameraMovement.FirstTouchContact.canceled += _ => EndMovements();
        _Controlls.CameraMovement.FirstTouchContact.canceled += _ => EndZoom();
        _Controlls.CameraMovement.SecondTouchContact.started += _ => StartZoom();
        _Controlls.CameraMovement.SecondTouchContact.canceled += _ => EndZoom();
    }
    private void StartMovement()
    {
        if (!IsPointerOverUI(_Controlls.CameraMovement.FirstTouchPosition.ReadValue<Vector2>()))
        {
            _touchStart = _mainCamera.ScreenToWorldPoint(_Controlls.CameraMovement.FirstTouchPosition.ReadValue<Vector2>());
            _MovementCoroutine = StartCoroutine(MoveCamera());
        }
    }
    private void EndMovements()
    {
        if (_MovementCoroutine != null)
        {
            StopCoroutine(_MovementCoroutine);
        }
        WasDraggingInLastMovement = _IsDragging;
        _IsDragging = false;
    }
    IEnumerator MoveCamera()
    {
        while (true)
        {
            if (_IsDragging && !_IsZoomin)
            {
                Vector3 direction = _touchStart - _mainCamera.ScreenToWorldPoint(_Controlls.CameraMovement.FirstTouchPosition.ReadValue<Vector2>());
                if (EstaDentroDelRombo(_cameraController.transform.position + direction))
                {
                    _cameraController.transform.position += direction;
                }
                else
                {
                    _cameraController.transform.position = ObtenerPosicionLimite(_cameraController.transform.position += direction);
                }
                //Debug.Log($"Touch (0): Touch Start {_touchStart} Position {_Controlls.CameraMovement.FirstTouchPosition.ReadValue<Vector2>()}, Delta Position {_Controlls.CameraMovement.FirstTouchDeltaPosition.ReadValue<Vector2>()}");
            }
            else
            {
                if (Vector2.Distance(_touchStart, _mainCamera.ScreenToWorldPoint(_Controlls.CameraMovement.FirstTouchPosition.ReadValue<Vector2>())) > MinDistanceToDetectDrag)
                {
                    _IsDragging = true;
                    _touchStart = _mainCamera.ScreenToWorldPoint(_Controlls.CameraMovement.FirstTouchPosition.ReadValue<Vector2>());
                }
            }
            yield return null;
        }
    }
    private void StartZoom()
    {
        if (!IsPointerOverUI(_Controlls.CameraMovement.SecondTouchPosition.ReadValue<Vector2>()))
        {
            _IsZoomin = true;
            _ZoomCoroutine = StartCoroutine(Zoom());
        }
    }
    private void EndZoom()
    {
        _IsZoomin = false;
        if (_ZoomCoroutine != null)
        {
            StopCoroutine(_ZoomCoroutine);
        }
    }
    IEnumerator Zoom()
    {
        while (true)
        {
            Vector2 touchZeroPrevPos = _Controlls.CameraMovement.FirstTouchPosition.ReadValue<Vector2>() - _Controlls.CameraMovement.FirstTouchDeltaPosition.ReadValue<Vector2>();
            Vector2 touchOnePrevPos = _Controlls.CameraMovement.SecondTouchPosition.ReadValue<Vector2>() - _Controlls.CameraMovement.SecondTouchDeltaPosition.ReadValue<Vector2>();
            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (_Controlls.CameraMovement.FirstTouchPosition.ReadValue<Vector2>() - _Controlls.CameraMovement.SecondTouchPosition.ReadValue<Vector2>()).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            _cameraController.m_Lens.OrthographicSize = Mathf.Clamp(_cameraController.m_Lens.OrthographicSize - difference * 0.01f, _maxZoom, _minZoom);
            yield return null;
        }
    }

    public bool IsPointerOverUI(Vector2 position)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = position;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        if (results.Count > 0)
        {
            if (results[0].gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                if (DebugMode)
                {
                    Debug.Log("Has tocado el elemento de UI: " + results[0].gameObject.name);
                }
                return true;
            }
            else
            {
                if (DebugMode)
                {
                    Debug.Log("Has tocado el GameObject: " + results[0].gameObject.name);
                }

                return false;

            }
        }
        else
        {
            if (DebugMode)
            {
                Debug.Log("No has tocado ningun objeto");
            }
            return false;
        }
    }
    bool EstaDentroDelRombo(Vector3 posicion)
    {
        // Calcular las esquinas del rombo
        Vector3 esquinaSuperior = new Vector3(0, +CameraLimits.y / 2, 0);
        Vector3 esquinaDerecha = new Vector3(+CameraLimits.x / 2, 0, 0);
        Vector3 esquinaInferior = new Vector3(0, -CameraLimits.y / 2, 0);
        Vector3 esquinaIzquierda = new Vector3(-CameraLimits.x / 2, 0, 0);

        // Verificar si la posición está dentro del rombo
        return EstaDentroDelTriangulo(Vector3.zero, esquinaSuperior, esquinaDerecha, posicion) ||
               EstaDentroDelTriangulo(Vector3.zero, esquinaDerecha, esquinaInferior, posicion) ||
               EstaDentroDelTriangulo(Vector3.zero, esquinaInferior, esquinaIzquierda, posicion) ||
               EstaDentroDelTriangulo(Vector3.zero, esquinaIzquierda, esquinaSuperior, posicion);
    }
    bool EstaDentroDelTriangulo(Vector3 a, Vector3 b, Vector3 c, Vector3 punto)
    {
        // Calcular las áreas de los triángulos formados por el punto y los vértices del triángulo
        float areaTotal = AreaTriangulo(a, b, c);
        float area1 = AreaTriangulo(a, b, punto);
        float area2 = AreaTriangulo(b, c, punto);
        float area3 = AreaTriangulo(c, a, punto);

        // Si la suma de las áreas de los triángulos es igual al área total, el punto está dentro del triángulo
        return Mathf.Approximately(areaTotal, area1 + area2 + area3);
    }
    float AreaTriangulo(Vector3 a, Vector3 b, Vector3 c)
    {
        return Mathf.Abs((a.x * (b.y - c.y) + b.x * (c.y - a.y) + c.x * (a.y - b.y)) / 2f);
    }
    Vector3 ObtenerPosicionLimite(Vector3 posicion)
    {
        // Calcular las esquinas del rombo
        Vector3 esquinaSuperior = new Vector3(0, +CameraLimits.y / 2, 0);
        Vector3 esquinaDerecha = new Vector3(+CameraLimits.x / 2, 0, 0);
        Vector3 esquinaInferior = new Vector3(0, -CameraLimits.y / 2, 0);
        Vector3 esquinaIzquierda = new Vector3(-CameraLimits.x / 2, 0, 0);

        // Calcular la dirección desde el centro del rombo hasta la posición actual del objeto
        Vector3 direccionObjeto = posicion - Vector3.zero;

        // Calcular la intersección con los bordes del rombo
        Vector3 interseccion = Vector3.zero;

        if (direccionObjeto.y >= 0)
        {
            // Intersección con los bordes superiores
            float signo = Mathf.Sign(direccionObjeto.x);
            if (signo < 0)
            {
                interseccion = CalcularInterseccion(esquinaIzquierda, esquinaSuperior, Vector2.zero, direccionObjeto * 100);
            }
            else
            {
                interseccion = CalcularInterseccion(esquinaDerecha, esquinaSuperior, Vector2.zero, direccionObjeto * 100);
            }
        }
        else
        {
            // Intersección con los bordes inferiores
            float signo = Mathf.Sign(direccionObjeto.x);
            if (signo < 0)
            {
                interseccion = CalcularInterseccion(esquinaIzquierda, esquinaInferior, Vector2.zero, direccionObjeto * 100);
            }
            else
            {
                interseccion = CalcularInterseccion(esquinaDerecha, esquinaInferior, Vector2.zero, direccionObjeto * 100);
            }
        }

        return interseccion + Vector3.back * 10;
    }
    public static Vector2 CalcularInterseccion(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        float x1 = p1.x, y1 = p1.y;
        float x2 = p2.x, y2 = p2.y;
        float x3 = p3.x, y3 = p3.y;
        float x4 = p4.x, y4 = p4.y;

        float denom = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);

        if (Mathf.Approximately(denom, 0))
        {
            // Las líneas son paralelas o coincidentes, no hay intersección.
            return Vector2.zero;
        }

        float t1 = ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / denom;
        float t2 = -((x1 - x2) * (y1 - y3) - (y1 - y2) * (x1 - x3)) / denom;

        if (t1 >= 0 && t1 <= 1 && t2 >= 0 && t2 <= 1)
        {
            // Las líneas se intersectan dentro de los segmentos.
            float x = x1 + t1 * (x2 - x1);
            float y = y1 + t1 * (y2 - y1);
            return new Vector2(x, y);
        }
        else
        {
            // Las líneas se intersectan fuera de los segmentos.
            return Vector2.zero;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        // Dibujar las líneas del cuadrado
        Vector3 esquinaSuperior = new Vector3(0, 0 + CameraLimits.y / 2, 0);
        Vector3 esquinaDerecha = new Vector3(0 + CameraLimits.x / 2, 0, 0);
        Vector3 esquinaInferior = new Vector3(0, 0 - CameraLimits.y / 2, 0);
        Vector3 esquinaIzquierda = new Vector3(0 - CameraLimits.x / 2, 0, 0);

        Gizmos.DrawLine(esquinaSuperior, esquinaDerecha);
        Gizmos.DrawLine(esquinaDerecha, esquinaInferior);
        Gizmos.DrawLine(esquinaInferior, esquinaIzquierda);
        Gizmos.DrawLine(esquinaIzquierda, esquinaSuperior);
    }



}
