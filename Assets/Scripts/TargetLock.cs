using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TargetLock : MonoBehaviour
{
    [Header("Objects")]
    [Space]
    [SerializeField] private Camera mainCamera;            // your main camera object.
    [SerializeField] private CinemachineFreeLook cinemachineFreeLook; //cinemachine free lock camera object.

    [Space]
    [Header("UI")]
    [SerializeField] private Image aimIcon;  // ui image of aim icon u can leave it null.
    [SerializeField] private GameObject enemyHealthBar;
    [SerializeField] private TMP_Text enemyNameText;
    [Space]
    [Header("Settings")]
    [Space]
    [SerializeField] private string enemyTag; // the enemies tag.
    [SerializeField] private KeyCode _Input;
    [SerializeField] private bool isManuallyTargeting;
    [SerializeField] private Vector2 targetLockOffset;
    [SerializeField] private float minDistance; // minimum distance to stop rotation if you get close to target
    [SerializeField] private float maxDistance;
    [SerializeField] private float maxDistanceFromTarget;
    [SerializeField] private HighlightManager highlightManager;
    public bool isTargeting;
    private GameObject currentEnemy;

    private float maxAngle;
    private Transform currentTarget;
    private float mouseX;
    private float mouseY;

    void Start()
    {
        highlightManager = this.GetComponent<HighlightManager>();
        maxAngle = 90f; // always 90 to target enemies in front of camera.
        cinemachineFreeLook.m_XAxis.m_InputAxisName = "";
        cinemachineFreeLook.m_YAxis.m_InputAxisName = "";
    }

    void Update()
    {
        if (!isTargeting)
        {
            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");
        }
        else
        {
            NewInputTarget(currentTarget);
        }

        if (aimIcon)
            aimIcon.gameObject.SetActive(isTargeting);

        cinemachineFreeLook.m_XAxis.m_InputAxisValue = mouseX;
        cinemachineFreeLook.m_YAxis.m_InputAxisValue = mouseY;

        if (Input.GetKeyDown(_Input))
        {
            AssignTarget();
        }


        if (isTargeting && currentTarget != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);
            if (distanceToTarget > maxDistanceFromTarget)
            {
                isTargeting = false;
                currentTarget = null;
                highlightManager.DeselectHighlight();
            }
        }

        // Check for mouse click to target an enemy.
        if (Input.GetMouseButtonDown(0)) // Change 0 to the appropriate mouse button if needed.
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag(enemyTag))
                {
                    // Clicked on an enemy, set it as the target.
                    currentEnemy = hit.collider.gameObject;
                    currentTarget = hit.transform;
                    isTargeting = true;
                    highlightManager.SelectedHighlight();
                    //enemyHealthBar.SetActive(true);
                    enemyNameText.text = currentTarget.GetComponent<Enemy>().enemyName;
                    //here is where i add the update health
                    currentTarget.GetComponent<Enemy>().SetupHealthBar();
                    currentTarget.GetComponent<Enemy>().ToggleHealthBarEnemyModeIcon();
                    currentTarget.GetComponent<Enemy>().ToggleVisibilityOnEnemy();

                }
                else
                {
                    // Clicked on something other than an enemy, deselect the current target.
                    currentTarget = null;
                    isTargeting = false;
                    if(highlightManager.CurrentlySelected() == true)
                    {
                        GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
                        enemy.GetComponent<Enemy>().ToggleVisibilityOffEnemy();
                        enemyNameText.text = "";
                        highlightManager.DeselectHighlight();

                    }

                }
            }
            else
            {
                // Clicked on empty space, deselect the current target.
                currentTarget = null;
                isTargeting = false;

            }


        }

    }


    public void EnemyDeathDeselect()
    {
        // Clicked on something other than an enemy, deselect the current target.
        currentTarget = null;
        isTargeting = false;
        if (highlightManager.CurrentlySelected() == true)
        {
            GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
            enemy.GetComponent<Enemy>().ToggleVisibilityOffEnemy();
            enemyNameText.text = "";
            highlightManager.DeselectHighlight();

        }
    }

    private void AssignTarget()
    {
        if (isTargeting)
        {
            // If already targeting, toggle off targeting and hide the health bar.
            isTargeting = false;
            currentTarget = null;
            highlightManager.DeselectHighlight();
            enemyNameText.text = "";

            // Toggle off the visibility of the health bar here.
            if (currentEnemy != null)
            {
                currentEnemy.GetComponent<Enemy>().ToggleVisibilityOffEnemy();
            }

            return;
        }

        if (ClosestTarget())
        {
            // If a target is found, toggle on targeting and show the health bar.
            currentTarget = ClosestTarget().transform;
            isTargeting = true;
            highlightManager.TabSelectedHighlight(ClosestTarget());

            enemyNameText.text = currentTarget.GetComponent<Enemy>().enemyName;

            // Toggle on the visibility of the health bar here.
            if (currentTarget != null)
            {
                currentTarget.GetComponent<Enemy>().SetupHealthBar();
                currentTarget.GetComponent<Enemy>().ToggleHealthBarEnemyModeIcon();
                currentTarget.GetComponent<Enemy>().ToggleVisibilityOnEnemy();
            }
        }
    }


    private void NewInputTarget(Transform target) // sets new input value.
    {
        if (!currentTarget) return;

        Vector3 viewPos = mainCamera.WorldToViewportPoint(target.position);

        if (aimIcon)
            aimIcon.transform.position = mainCamera.WorldToScreenPoint(target.position);

        if ((target.position - transform.position).magnitude < minDistance) return;
        mouseX = (viewPos.x - 0.5f + targetLockOffset.x) * 3f;              // adjust the [ 3f ] value to make it faster or  slower
        mouseY = (viewPos.y - 0.5f + targetLockOffset.y) * 3f;              
    }


    private GameObject ClosestTarget() // this is modified func from unity Docs ( Gets Closest Object with Tag ). 
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag(enemyTag);
        GameObject closest = null;
        float distance = maxDistance;
        float currAngle = maxAngle;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.magnitude;
            if (curDistance < distance)
            {
                Vector3 viewPos = mainCamera.WorldToViewportPoint(go.transform.position);
                Vector2 newPos = new Vector3(viewPos.x - 0.5f, viewPos.y - 0.5f);
                if (Vector3.Angle(diff.normalized, mainCamera.transform.forward) < maxAngle)
                {
                    closest = go;
                    currAngle = Vector3.Angle(diff.normalized, mainCamera.transform.forward.normalized);
                    distance = curDistance;
                    currentEnemy = closest; //This is to store the enemy for projectile to go towards
                }
            }
        }
        return closest;
    }


    public GameObject TheCurrentEnemy()
    {
        return currentEnemy;
    }




    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }

}
