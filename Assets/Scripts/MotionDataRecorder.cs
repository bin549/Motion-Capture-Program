using UnityEngine;
using System;
using System.IO;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif

[DefaultExecutionOrder(1)]
public class MotionDataRecorder : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    [SerializeField] public bool recording;
    [SerializeField] protected int FrameIndex;

    [SerializeField] private MotionDataSettings.Rootbonesystem _rootBoneSystem = MotionDataSettings.Rootbonesystem.Objectroot;
    [SerializeField] private HumanBodyBones _targetRootBone = HumanBodyBones.Hips;
    [SerializeField] private HumanBodyBones IK_LeftFootBone = HumanBodyBones.LeftFoot;
    [SerializeField] private HumanBodyBones IK_RightFootBone = HumanBodyBones.RightFoot;

    protected HumanoidPoses Poses;
    protected float RecordedTime;
    protected float StartTime;

    private HumanPose _currentPose;
    private HumanPoseHandler _poseHandler;
    public Action OnRecordStart;
    public Action OnRecordEnd;
    public Avatar avatar;

    public float TargetFPS = 60.0f;


    [SerializeField] private AppSettings appSettings;

    private void Awake()
    {
        appSettings = GameObject.FindObjectOfType<AppSettings>();
    }

    private void Start()
    {

        avatar = appSettings.GetAvatar();
        _animator = appSettings.GetAvatarAnimator();
        if (_animator == null)
        {
            Destroy(this);
            return;
        }

        _poseHandler = new HumanPoseHandler(_animator.avatar, _animator.transform);
    }

    private void LateUpdate()
    {
        if (!recording)
        {
            return;
        }
        RecordedTime = Time.time - StartTime;
        if (TargetFPS != 0.0f)
        {
            var nextTime = (1.0f * (FrameIndex + 1)) / TargetFPS;
            if (nextTime > RecordedTime)
            {
                return;
            }
            if (FrameIndex % TargetFPS == 0)
            {
                print("Motion_FPS=" + 1 / (RecordedTime / FrameIndex));
            }
        }
        else
        {
            if (Time.frameCount % Application.targetFrameRate == 0)
            {
                print("Motion_FPS=" + 1 / Time.deltaTime);
            }
        }
        _poseHandler.GetHumanPose(ref _currentPose);
        var serializedPose = new HumanoidPoses.SerializeHumanoidPose();
        switch (_rootBoneSystem)
        {
            case MotionDataSettings.Rootbonesystem.Objectroot:
                serializedPose.BodyRootPosition = _animator.transform.localPosition;
                serializedPose.BodyRootRotation = _animator.transform.localRotation;
                break;
            case MotionDataSettings.Rootbonesystem.Hipbone:
                serializedPose.BodyRootPosition = _animator.GetBoneTransform(_targetRootBone).position;
                serializedPose.BodyRootRotation = _animator.GetBoneTransform(_targetRootBone).rotation;
                Debug.LogWarning(_animator.GetBoneTransform(_targetRootBone).position);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        var bodyTQ = new TQ(_currentPose.bodyPosition, _currentPose.bodyRotation);
        var LeftFootTQ = new TQ(_animator.GetBoneTransform(IK_LeftFootBone).position, _animator.GetBoneTransform(IK_LeftFootBone).rotation);
        var RightFootTQ = new TQ(_animator.GetBoneTransform(IK_RightFootBone).position, _animator.GetBoneTransform(IK_RightFootBone).rotation);
        LeftFootTQ = AvatarUtility.GetIKGoalTQ(_animator.avatar, _animator.humanScale, AvatarIKGoal.LeftFoot, bodyTQ, LeftFootTQ);
        RightFootTQ = AvatarUtility.GetIKGoalTQ(_animator.avatar, _animator.humanScale, AvatarIKGoal.RightFoot, bodyTQ, RightFootTQ);
        serializedPose.BodyPosition = bodyTQ.t;
        serializedPose.BodyRotation = bodyTQ.q;
        serializedPose.LeftfootIK_Pos = LeftFootTQ.t;
        serializedPose.LeftfootIK_Rot = LeftFootTQ.q;
        serializedPose.RightfootIK_Pos = RightFootTQ.t;
        serializedPose.RightfootIK_Rot = RightFootTQ.q;
        serializedPose.FrameCount = FrameIndex;
        serializedPose.Muscles = new float[_currentPose.muscles.Length];
        serializedPose.Time = RecordedTime;
        for (int i = 0; i < serializedPose.Muscles.Length; i++)
        {
            serializedPose.Muscles[i] = _currentPose.muscles[i];
        }

        SetHumanBoneTransformToHumanoidPoses(_animator, ref serializedPose);

        Poses.Poses.Add(serializedPose);
        FrameIndex++;
    }

    public void RecordStart()
    {
        if (recording)
        {
            return;
        }
        Poses = ScriptableObject.CreateInstance<HumanoidPoses>();
        if (OnRecordStart != null)
        {
            OnRecordStart();
        }
        OnRecordEnd += WriteAnimationFile;
        recording = true;
        RecordedTime = 0f;
        StartTime = Time.time;
        FrameIndex = 0;
    }

    public void RecordEnd()
    {
        if (!recording)
        {
            return;
        }
        if (OnRecordEnd != null)
        {
            OnRecordEnd();
        }
        OnRecordEnd -= WriteAnimationFile;
        recording = false;
    }

    private static void SetHumanBoneTransformToHumanoidPoses(Animator animator, ref HumanoidPoses.SerializeHumanoidPose pose)
    {
        HumanBodyBones[] values = Enum.GetValues(typeof(HumanBodyBones)) as HumanBodyBones[];
        foreach (HumanBodyBones b in values)
        {
            if (b < 0 || b >= HumanBodyBones.LastBone)
            {
                continue;
            }

            Transform t = animator.GetBoneTransform(b);
            if (t != null)
            {
                var bone = new HumanoidPoses.SerializeHumanoidPose.HumanoidBone();
                bone.Set(animator.transform, t);
                pose.HumanoidBones.Add(bone);
            }
        }
    }

    /*  protected virtual void WriteAnimationFile()
      {
string path = Application.dataPath + @"/Resources";
          string filename = string.Format("/RecordMotion_{0}{1:yyyy_MM_dd_HH_mm_ss}.asset", _animator.name, DateTime.Now);

          Debug.Log(  path+filename);
          var uniqueAssetPath = AssetDatabase.GenerateUniqueAssetPath(path+filename);
*/
    /*
    #if UNITY_EDITOR
                FolderUtils.SafeCreateDirectory("Assets/Resources");

                var path = string.Format("Assets/Resources/RecordMotion_{0}{1:yyyy_MM_dd_HH_mm_ss}.asset", _animator.name, DateTime.Now);
                var uniqueAssetPath = AssetDatabase.GenerateUniqueAssetPath(path);

                AssetDatabase.CreateAsset(Poses, uniqueAssetPath);
                AssetDatabase.Refresh();
                #endif

                StartTime = Time.time;
                RecordedTime = 0f;
                FrameIndex = 0;
            }
    */
    protected virtual void WriteAnimationFile()
    {
        // Poses.SaveAnim();
    }

    public Animator CharacterAnimator
    {
        get { return _animator; }
    }

    public class TQ
    {
        public TQ(Vector3 translation, Quaternion rotation)
        {
            t = translation;
            q = rotation;
        }
        public Vector3 t;
        public Quaternion q;
    }

    public class AvatarUtility
    {
        static public TQ GetIKGoalTQ(UnityEngine.Avatar avatar, float humanScale, AvatarIKGoal avatarIKGoal, TQ animatorBodyPositionRotation, TQ skeletonTQ)
        {
            int humanId = (int)HumanIDFromAvatarIKGoal(avatarIKGoal);
            if (humanId == (int)HumanBodyBones.LastBone)
                throw new InvalidOperationException("Invalid human id.");
            MethodInfo methodGetAxisLength = typeof(UnityEngine.Avatar).GetMethod("GetAxisLength", BindingFlags.Instance | BindingFlags.NonPublic);
            if (methodGetAxisLength == null)
                throw new InvalidOperationException("Cannot find GetAxisLength method.");
            MethodInfo methodGetPostRotation = typeof(UnityEngine.Avatar).GetMethod("GetPostRotation", BindingFlags.Instance | BindingFlags.NonPublic);
            if (methodGetPostRotation == null)
                throw new InvalidOperationException("Cannot find GetPostRotation method.");
            Quaternion postRotation = (Quaternion)methodGetPostRotation.Invoke(avatar, new object[] { humanId });
            var goalTQ = new TQ(skeletonTQ.t, skeletonTQ.q * postRotation);
            if (avatarIKGoal == AvatarIKGoal.LeftFoot || avatarIKGoal == AvatarIKGoal.RightFoot)
            {
                float axislength = (float)methodGetAxisLength.Invoke(avatar, new object[] { humanId });
                Vector3 footBottom = new Vector3(axislength, 0, 0);
                goalTQ.t += (goalTQ.q * footBottom);
            }
            Quaternion invRootQ = Quaternion.Inverse(animatorBodyPositionRotation.q);
            goalTQ.t = invRootQ * (goalTQ.t - animatorBodyPositionRotation.t);
            goalTQ.q = invRootQ * goalTQ.q;
            goalTQ.t /= humanScale;

            return goalTQ;
        }

        static public HumanBodyBones HumanIDFromAvatarIKGoal(AvatarIKGoal avatarIKGoal)
        {
            HumanBodyBones humanId = HumanBodyBones.LastBone;
            switch (avatarIKGoal)
            {
                case AvatarIKGoal.LeftFoot: humanId = HumanBodyBones.LeftFoot; break;
                case AvatarIKGoal.RightFoot: humanId = HumanBodyBones.RightFoot; break;
                case AvatarIKGoal.LeftHand: humanId = HumanBodyBones.LeftHand; break;
                case AvatarIKGoal.RightHand: humanId = HumanBodyBones.RightHand; break;
            }
            return humanId;
        }
    }
}
