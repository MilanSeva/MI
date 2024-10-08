import type { ColDef, ColGroupDef, ColumnEventType, ComponentSelector, ToolPanelColumnCompParams } from 'ag-grid-community';
import { Component } from 'ag-grid-community';
export declare class AgPrimaryCols extends Component {
    private readonly primaryColsHeaderPanel;
    private readonly primaryColsListPanel;
    private allowDragging;
    private params;
    private eventType;
    private positionableFeature;
    constructor();
    init(allowDragging: boolean, params: ToolPanelColumnCompParams, eventType: ColumnEventType): void;
    toggleResizable(resizable: boolean): void;
    onExpandAll(): void;
    onCollapseAll(): void;
    expandGroups(groupIds?: string[]): void;
    collapseGroups(groupIds?: string[]): void;
    setColumnLayout(colDefs: (ColDef | ColGroupDef)[]): void;
    private onFilterChanged;
    syncLayoutWithGrid(): void;
    private onSelectAll;
    private onUnselectAll;
    private onGroupExpanded;
    private onSelectionChange;
    getExpandedGroups(): string[];
}
export declare const AgPrimaryColsSelector: ComponentSelector;
