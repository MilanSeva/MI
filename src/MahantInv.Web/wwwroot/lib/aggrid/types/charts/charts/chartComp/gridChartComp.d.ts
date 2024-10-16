import type { BeanCollection, ChartModel, ChartToolPanelName, ChartType, IAggFunc, PartialCellRange, SeriesChartType, UpdateChartParams } from 'ag-grid-community';
import { Component } from 'ag-grid-community';
import type { AgChartInstance, AgChartThemeOverrides, AgChartThemePalette } from 'ag-charts-community';
import type { CrossFilteringContext } from '../chartService';
export interface GridChartParams {
    chartId: string;
    pivotChart?: boolean;
    cellRange: PartialCellRange;
    chartType: ChartType;
    chartThemeName?: string;
    insideDialog: boolean;
    focusDialogOnOpen?: boolean;
    suppressChartRanges?: boolean;
    switchCategorySeries?: boolean;
    aggFunc?: string | IAggFunc;
    chartThemeOverrides?: AgChartThemeOverrides;
    unlinkChart?: boolean;
    crossFiltering?: boolean;
    crossFilteringContext: CrossFilteringContext;
    chartOptionsToRestore?: AgChartThemeOverrides;
    chartPaletteToRestore?: AgChartThemePalette;
    seriesChartTypes?: SeriesChartType[];
    crossFilteringResetCallback?: () => void;
}
export declare class GridChartComp extends Component {
    private crossFilterService;
    private chartTranslationService;
    private chartMenuService;
    private focusService;
    private popupService;
    wireBeans(beans: BeanCollection): void;
    private readonly eChart;
    private readonly eChartContainer;
    private readonly eMenuContainer;
    private readonly eEmpty;
    private chartMenu;
    private chartDialog;
    private chartController;
    private chartOptionsService;
    private chartMenuContext;
    private chartProxy;
    private chartType;
    private chartEmpty;
    private readonly params;
    private onDestroyColorSchemeChangeListener;
    constructor(params: GridChartParams);
    postConstruct(): void;
    private createChart;
    private createMenuContext;
    private getChartThemeName;
    private getChartThemes;
    private getGridOptionsChartThemeOverrides;
    private static createChartProxy;
    private addDialog;
    private getBestDialogSize;
    private addMenu;
    update(params?: UpdateChartParams): void;
    private updateChart;
    private chartTypeChanged;
    getChartModel(): ChartModel;
    getChartImageDataURL(fileFormat?: string): string;
    private handleEmptyChart;
    downloadChart(dimensions?: {
        width: number;
        height: number;
    }, fileName?: string, fileFormat?: string): void;
    openChartToolPanel(panel?: ChartToolPanelName): void;
    closeChartToolPanel(): void;
    getChartId(): string;
    getUnderlyingChart(): AgChartInstance<import("ag-charts-community").AgChartOptions>;
    crossFilteringReset(): void;
    private setActiveChartCellRange;
    private getThemeName;
    private getAllKeysInObjects;
    private validateCustomThemes;
    private reactivePropertyUpdate;
    private raiseChartCreatedEvent;
    private raiseChartDestroyedEvent;
    destroy(): void;
}
