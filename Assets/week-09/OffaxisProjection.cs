using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this class presents an example of how we can create a projection image that
// adapts to the point of view of the camera.
// This can be used in virtual production, where a background screen is used
// to render the virtual content and real-time, using this virtual camera, 
// and a real camera captures the image from unity in the background, as a
// projection or LED screen, with actors in the foreground
[ExecuteInEditMode]
public class OffaxisProjection : MonoBehaviour
{
    [SerializeField] Transform screenBottomLeft, screenTopRight;
    [SerializeField] Camera cam;

    void OnValidate()
    {
        if (cam == null)
            cam = gameObject.GetComponent<Camera>();
        if (cam == null)
            Debug.LogError("[OffaxisProjection] there is no 'camera' object defined!");
        if (screenBottomLeft == null || screenTopRight == null)
            Debug.LogError("[OffaxisProjection] 'screenBottomLeft' and 'screenTopRight' must be assigned!");
    }

    void Start()
    {
        OnValidate();
    }

    void Update()
    {
        if (screenBottomLeft == null || screenTopRight == null || cam == null)
            return;

        // DO NOT ROTATE THE CAMERA OR THE SCREEN
        // to make things simple, I don't support camera and screen rotation
        float near, far, left, right, top, bottom;

        near = (screenBottomLeft.position - cam.transform.position).z;
        far = cam.farClipPlane;
        left = (screenBottomLeft.position - cam.transform.position).x;
        bottom = (screenBottomLeft.position - cam.transform.position).y;
        right = (screenTopRight.position - cam.transform.position).x;
        top = (screenTopRight.position - cam.transform.position).y;

        // The perspective projection matrix, same as explained in
        // http://www.songho.ca/opengl/gl_projectionmatrix.html
        // m00=2*n/(r-l)    m01=0            m02=(r+l)/(r-l)     m03=0
        // m10=0            m11=2*n/(t-b)    m12=(t+b)/(t-b)     m13=0
        // m20=0            m21=0            m22=-(f+n)/(f-n)    m23=-2*f*n/(f-n)
        // m30=0            m31=0            m32=-1              m33=0
        Matrix4x4 projection = Matrix4x4.identity;
        // these three lines scale the scene for the camera
        projection.m00 = 2 * near / (right - left);
        projection.m11 = 2 * near / (top - bottom);
        projection.m22 = -(far + near) / (far - near);
        // this translate along the z axis (depth)
        projection.m23 = -2 * far * near / (far - near);
        // these are the two line responsible for the offaxis projection in the
        // perspective projection matrix
        projection.m02 = (right + left) / (right - left);
        projection.m12 = (top + bottom) / (top - bottom);
        // perspective projection components
        projection.m32 = -1;
        projection.m33 = 0;

        // Unity has a built-in function that does all modifications above for us ;)
        //Matrix4x4 projection = Matrix4x4.Frustum(left, right, bottom, top, near, far);

        cam.projectionMatrix = projection;
        cam.nearClipPlane = near;

    }
}
