using System.Collections.Generic;
using UnityEngine;

public class AimAssist : MonoBehaviour
{
    // Rewritten and forked version of Mr Giff's GML code by ugurevren for Unity 2D.
    // The intent here is to apply a correction to the player's aim without wrestling control away from them in the form of hard "snapping."
    // This process is an interpretation of the work done by @t3ssel8r in this excellent video demonstrating his own research into this problem: https://www.youtube.com/watch?v=yGci-Lb87zs

    #region VARIABLES

    [Tooltip("The range[0-1] of tolerance before correction is applied")] [Range(0f, 1f)] [SerializeField]
    private float _maxCorrection; // 0.2 is a good value in my case (Top Down Shooter)

    [Tooltip("The maximum number of degrees applied as a correction to the player's aim")] [SerializeField]
    private float _maxAngle; // 10 is a good value in my case (Top Down Shooter)

    [Tooltip("The amount[0-1] of smoothing to apply to the interpolation")] [Range(0f, 1f)] [SerializeField]
    private float
        _smoothing; // 0 being "snappy" and 1 being "smooth"| 0.5 is a good value in my case (Top Down Shooter)

    [SerializeField] private Transform[] _enemies; // Assign the GameObjects you want to check in the Inspector
    private List<Transform> _targets = new List<Transform>(); // List of targets that are within the player's view

    // NOTE: If you will use this script in your player GameObject, you can take the player's transform by inspector or GetComponent<Transform>() in Start() function.
    
    // EXAMPLE USAGE : If you are using cursor for aiming, you can use SMTH like this.
    //---------------------------------------------------------------------------------------------------------------------
    //  float distanceToCursor = Vector3.Distance(playerPosition, cursorPosition); // Get the distance to the cursor
    //  float angle = Mathf.Atan2(directionToCursor.y, directionToCursor.x); // Get the angle to the cursor
    //  float assistedAngle=_aimAssist.AssistedAim(angle,player.transform); // Apply the correction
    //  directionToCursor = new Vector3(Mathf.Cos(assistedAngle), Mathf.Sin(assistedAngle), 0); // Get the direction to the cursor
    //  player.transform.up = directionToCursor; // Rotate the player towards the cursor
    //  player.transform.eulerAngles = new Vector3(0, 0, _upperBody.transform.eulerAngles.z); // If u don't use this line, player will be flipped
    //  cursor.position = playerPosition + directionToCursor * distanceToCursor; // Change the cursor's position
    //---------------------------------------------------------------------------------------------------------------------
    
    #endregion

    #region UNITY FUNCTIONS

    private void Update()
    {
        // Update the list of targets based on visibility
        foreach (var obj in _enemies)
        {
            if (IsGameObjectInView(obj) && !_targets.Contains(obj))
            {
                _targets.Add(obj);
            }
            else if (_targets.Contains(obj) && !IsGameObjectInView(obj))
            {
                _targets.Remove(obj);
            }
        }
    }

    #endregion

    #region PUBLIC_FUNCTIONS

    // This function is called in the player's script to apply the correction to the player's aim
    public float AssistedAim(float playerAimAngle, Transform player)
    {
        var assistedAim = playerAimAngle;

        var allCoords = TakeAllCoordinates();
        var arrOfX = allCoords[0];
        var arrOfY = allCoords[1];

        var bestDiff = Mathf.Infinity;
        var bestAngle = 0f;

        for (var i = 0; i < arrOfX.Length; i++)
        {
            var dir = new Vector2(arrOfX[i] - (player.position.x),
                arrOfY[i] - (player.position.y)); // Get the direction to the target
            //var dir = new Vector2(arrOfX[i] - (_player.position.x+ _offset.x), arrOfY[i] - (_player.position.y+ _offset.y)); // If you needed offset.
            var angle = Mathf.Atan2(dir.y, dir.x); // Get the angle to the target
            var diff = Mathf.Abs(Mathf.DeltaAngle(playerAimAngle,
                angle)); // Difference between the player's aim angle and the angle to the target

            // If the difference is within the tolerance and is the best so far, update the best angle
            if (diff <= _maxAngle)
            {
                bestDiff = diff;
                bestAngle = angle;
            }

            break;
        }

        // If the best angle is within the tolerance, apply the correction
        if (bestDiff <= _maxAngle)
        {
            var correction =
                _maxCorrection * (1 - (bestDiff / _maxAngle)); // Calculate the correction based on the difference
            var targetDirection =
                playerAimAngle -
                Mathf.Sign(Mathf.DeltaAngle(playerAimAngle, bestAngle)) * correction; // Calculate the target direction

            // If the player's aim angle is within the correction range, interpolate towards the target direction
            if (Mathf.Abs(Mathf.DeltaAngle(playerAimAngle, bestAngle)) < _maxCorrection / 1.6f)
            {
                assistedAim = Mathf.Lerp(playerAimAngle, bestAngle, SmoothStep(_smoothing));
            }
            else
            {
                assistedAim = playerAimAngle;
            }
        }

        return assistedAim;
    }

    // Use this function if you want to apply a different smoothing value for each enemy or want to change aim assist's correction from settings, maybe?
    public float AssistedAim(float playerAimAngle, Transform player, float maxCorrection, float maxAngle,
        float smoothing)
    {
        var assistedAim = playerAimAngle;

        var allCoords = TakeAllCoordinates();
        var arrOfX = allCoords[0];
        var arrOfY = allCoords[1];

        var bestDiff = Mathf.Infinity;
        var bestAngle = 0f;

        for (var i = 0; i < arrOfX.Length; i++)
        {
            var dir = new Vector2(arrOfX[i] - (player.position.x),
                arrOfY[i] - (player.position.y)); // Get the direction to the target
            //var dir = new Vector2(arrOfX[i] - (_player.position.x+ _offset.x), arrOfY[i] - (_player.position.y+ _offset.y)); // If you needed offset.
            var angle = Mathf.Atan2(dir.y, dir.x); // Get the angle to the target
            var diff = Mathf.Abs(Mathf.DeltaAngle(playerAimAngle,
                angle)); // Difference between the player's aim angle and the angle to the target

            // If the difference is within the tolerance and is the best so far, update the best angle
            if (diff <= maxAngle)
            {
                bestDiff = diff;
                bestAngle = angle;
            }

            break;
        }

        // If the best angle is within the tolerance, apply the correction
        if (bestDiff <= maxAngle)
        {
            var correction =
                maxCorrection * (1 - (bestDiff / maxAngle)); // Calculate the correction based on the difference
            var targetDirection =
                playerAimAngle -
                Mathf.Sign(Mathf.DeltaAngle(playerAimAngle, bestAngle)) * correction; // Calculate the target direction

            // If the player's aim angle is within the correction range, interpolate towards the target direction
            if (Mathf.Abs(Mathf.DeltaAngle(playerAimAngle, bestAngle)) < maxCorrection / 1.6f)
            {
                assistedAim = Mathf.Lerp(playerAimAngle, bestAngle, SmoothStep(smoothing));
            }
            else
            {
                assistedAim = playerAimAngle;
            }
        }

        return assistedAim;
    }

    #endregion

    #region PRIVATE_FUNCTIONS

    // Check if the GameObject obj is within the camera's view
    private bool IsGameObjectInView(Transform obj)
    {
        //NOTE: If you have more than one cam you should give reference to the cam you want to use
        var cam = Camera.main;
        var viewportPoint = cam.WorldToViewportPoint(obj.position);

        return viewportPoint.x >= 0 && viewportPoint.x <= 1 &&
               viewportPoint.y >= 0 && viewportPoint.y <= 1 &&
               viewportPoint.z > 0;
    }

    // Get the x and y coordinates of each enemy id in a given list and return two arrays within an a single 2D array.
    private float[][] TakeAllCoordinates()
    {
        var _arr_of_x = new float[_targets.Count];
        var _arr_of_y = new float[_targets.Count];
        for (var _i = 0; _i < _targets.Count; _i++)
        {
            _arr_of_x[_i] = _targets[_i].position.x;
            _arr_of_y[_i] = _targets[_i].position.y;
        }

        return new float[][] { _arr_of_x, _arr_of_y };
    }

    // Monotone Cubic Interpolation
    private float SmoothStep(float x)
    {
        var t = Mathf.Clamp(x, 0, 1);
        return t * t * (3 - 2 * t);
    }

    #endregion
}