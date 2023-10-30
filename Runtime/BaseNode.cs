using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace NodeGraph
{
    public abstract partial class BaseNode : ScriptableObject
    {
        // Maybe move this into the noedgraph?
        [SerializeField]
        private Rect position = new Rect();

        public Rect Position { get => position; set => position = value; }

        public RenderTexture PreviewTexture { get; private set; }

        public NodeGraph Graph { get; set; }

        public virtual void CreatePreviewTexture(Vector2Int resolution)
        {
            PreviewTexture = new RenderTexture(resolution.x, resolution.y, 0, RenderTextureFormat.ARGB32)
            {
                enableRandomWrite = true,
                hideFlags = HideFlags.HideAndDontSave,
            };
            PreviewTexture.Create();
        }

        public virtual void NodeChanged() { }

        public virtual bool HasPreviewTexture => false;

        /// <summary>
        /// Called when the Node is initialized. Use this to create/load persistent resources etc
        /// </summary>
        public virtual void Initialize() { }

        /// <summary>
        /// Called when the Node is decativated. Use this to release persistent resources
        /// </summary>
        public virtual void Cleanup()
        {
            if (PreviewTexture != null)
            {
                Object.DestroyImmediate(PreviewTexture);
            }
        }

        // Node Utility functions
        public bool NodeIsConnected(string fieldName) => GetConnectedNode(fieldName) != null;

        // Can be overridden to perform custom logic when nodes are updated
        public virtual void OnUpdateValues()
        {

        }

        // Used for relay nodes
        public virtual bool TryGetAdditionalNode(out BaseNode node)
        {
            node = null;
            return false;
        }

        public virtual void PreUpdateNodeOrder()
        {
        }

        // Below methods are code-gen'd. They should be abstract, but this causes compilation errors when codegen hasn't run
        public virtual T GetValueClass<T>(string fieldName) where T : class => default;

        public virtual void UpdateValues() => OnUpdateValues();

        public virtual void SetConnectedNode(string inputField, BaseNode outputNode, string outputField) { }

        public virtual BaseNode GetConnectedNode(string fieldName, out string connectedFieldName)
        {
            connectedFieldName = default;
            return default;
        }

        /// <summary>
        /// Returns the size of an array with fieldName.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public virtual int GetArraySize(string fieldName) => default;

        public BaseNode GetConnectedNode(string fieldName) => GetConnectedNode(fieldName, out _);

        public virtual void SetConnectedNodeArray(string inputField, BaseNode outputNode, string outputField, int arrayIndex) { }

        public virtual BaseNode GetConnectedNodeArray(string fieldName, int arrayIndex, out string connectedFieldName)
        {
            connectedFieldName = default;
            return default;
        }

        public virtual int GetNodeCount() => default;

        public virtual int GetNodeArrayCount() => default;

        public virtual int GetNodeArrayElementCount(int index) => default;

        public virtual BaseNode GetNodeAtIndex(int index) => default;

        public virtual BaseNode GetNodeAtArrayIndex(int arrayIndex, int elementIndex) => default;

        private Exception GetException(string fieldName) => new NotImplementedException($"{GetType()}.{fieldName}");

        // Todo: Codegen these
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual RenderTargetBinding GetValueRenderTargetBinding(string fieldName) => throw GetException(fieldName);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual RenderTargetIdentifier GetValueRenderTargetIdentifier(string fieldName) => throw GetException(fieldName);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual bool GetValueBoolean(string fieldName) => throw GetException(fieldName);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual CullingResults GetValueCullingResults(string fieldName) => throw GetException(fieldName);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Vector2Int GetValueVector2Int(string fieldName) => throw GetException(fieldName);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Vector2 GetValueVector2(string fieldName) => throw GetException(fieldName);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual int GetValueInt32(string fieldName) => throw GetException(fieldName);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual float GetValueSingle(string fieldName) => throw GetException(fieldName);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Vector4 GetValueVector4(string fieldName) => throw GetException(fieldName);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Color GetValueColor(string fieldName) => throw GetException(fieldName);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual T GetValueT<T>(string fieldName) => throw GetException(fieldName);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual AttachmentDescriptor GetValueAttachmentDescriptor(string fieldName) => throw GetException(fieldName);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual NodeConnection GetValueNodeConnection(string fieldName) => throw GetException(fieldName);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual RenderStateBlock GetValueRenderStateBlock(string fieldName) => throw GetException(fieldName);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual StencilState GetValueStencilState(string fieldName) => throw GetException(fieldName);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ShadowSplitData GetValueShadowSplitData(string fieldName) => throw GetException(fieldName);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ShadowDrawingSettings GetValueShadowDrawingSettings(string fieldName) => throw GetException(fieldName);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual GpuInstanceBuffers GetValueGpuInstanceBuffers(string fieldName) => throw GetException(fieldName);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Matrix4x4 GetValueMatrix4x4(string fieldName) => throw GetException(fieldName);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual BuiltinRenderTextureType GetValueBuiltinRenderTextureType(string fieldName) => throw GetException(fieldName);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual Vector3 GetValueVector3(string fieldName) => throw GetException(fieldName);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual CullingPlanes GetValueCullingPlanes(string fieldName) => throw GetException(fieldName);
    }
}