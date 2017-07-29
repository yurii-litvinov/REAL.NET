using WPF_Editor.ViewModels.Helpers;

namespace WPF_Editor.ViewModels.Interfaces
{
    public interface ISceneMediatorViewModel
    {
        MetamodelElement GetSelectedMetamodelType();
        ModelNode GetModelNode(MetamodelNode metaNode);
        ModelEdge GetModelEdge(MetamodelEdge metaEdge, ModelNode source, ModelNode target);
    }
}