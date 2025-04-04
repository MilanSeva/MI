import type { BeanCollection, IRowModel, IViewportDatasource, NamedBean, RowBounds, RowModelType } from 'ag-grid-community';
import { BeanStub, RowNode } from 'ag-grid-community';
export declare class ViewportRowModel extends BeanStub implements NamedBean, IRowModel {
    beanName: "rowModel";
    private rowRenderer;
    private focusService;
    private beans;
    wireBeans(beans: BeanCollection): void;
    private firstRow;
    private lastRow;
    private rowCount;
    private rowNodesByIndex;
    private rowHeight;
    private viewportDatasource;
    ensureRowHeightsValid(startPixel: number, endPixel: number, startLimitIndex: number, endLimitIndex: number): boolean;
    postConstruct(): void;
    start(): void;
    isLastRowIndexKnown(): boolean;
    destroy(): void;
    private destroyDatasource;
    private updateDatasource;
    private getViewportRowModelPageSize;
    private getViewportRowModelBufferSize;
    private calculateFirstRow;
    private calculateLastRow;
    private onViewportChanged;
    purgeRowsNotInViewport(): void;
    private isRowFocused;
    setViewportDatasource(viewportDatasource: IViewportDatasource): void;
    getType(): RowModelType;
    getRow(rowIndex: number): RowNode;
    getRowNode(id: string): RowNode | undefined;
    getRowCount(): number;
    getRowIndexAtPixel(pixel: number): number;
    getRowBounds(index: number): RowBounds;
    private updateRowHeights;
    getTopLevelRowCount(): number;
    getTopLevelRowDisplayedIndex(topLevelIndex: number): number;
    isEmpty(): boolean;
    isRowsToRender(): boolean;
    getNodesInRangeForSelection(firstInRange: RowNode, lastInRange: RowNode): RowNode[];
    forEachNode(callback: (rowNode: RowNode, index: number) => void): void;
    private setRowData;
    private createBlankRowNode;
    setRowCount(rowCount: number, keepRenderedRows?: boolean): void;
    isRowPresent(rowNode: RowNode): boolean;
}
