Create code for:
  Add Module
  Add Group
  Add List
  Add Document

Create warnings when modules in groups, lists or documents does not exist in the modules view.
Create warnings when groups in lists or documents does not exist in the group view.
Create warnings when lists in documents does not exist in the list view.

Look at how tag is generated and store everything in it - update via oncollapse/onexpand, via
new NodeTag.IsOpen property

Update the tag when the folder is open/closed, somehow give the tag an TreeNode.IsOpen, but it
requires access to the treeview, so the treeview must be added to the tag.

Add displaytext to the tag type.
Add path to the tag type.

Add the module_path and document_path in the property settings.

When dropping a node, first update the data then update the view.

Store all modules in a global list by name
Store all modules in a global list by basename
Store all modules in a global list by displaytext

// The icon of a folder is always
AddNode(TreeView tv, TreeView src, TreeNode node) {
    NodeTag tag = node.Tag as NodeTag;
    // Test Basename without extension
    if(exists_child_of_parent_with_same_displaytext(TreeView tv, node))
       return;

    if(tv == TvModules) {
        // Files dropped to the modules treeview (from explorer) ... handle elswhere
        return;
    }

    if(tv == TvGroups) {
        if(tvsrc != TvModules)
            return;
        // todo test if the type of the node is StandardModule.
        // (otherwise it is potentially a folder to be dropped into)
        //
        // todo add the node
        // todo you can not add to a module folder
        // todo add module or module folder
        // todo add option to turn copies of a module folder into a group in the group view.
        // todo add option to turn copies of a module folder into a list in the list view.
        // todo add option to turn copies of a module folder into a document folder in the document view.
        return;
    }
    if(tv == TvLists) {
        if(tvsrc != TvModules && tvsrc != TvGroups)
            return;
        // todo add module or module folder or group
        return;
    }
    if(tv == TvDocuments) {
        if(tvsrc != TvModules && tvsrc != TvGroups && tvsrc != TvLists) 
            return;
        // todo add module or module folder or group or list
        return;
    }
}

