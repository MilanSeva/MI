import type { ChartType } from 'ag-grid-community';
import { MiniChartWithAxes } from '../miniChartWithAxes';
export declare class MiniLine extends MiniChartWithAxes {
    static chartType: ChartType;
    private readonly lines;
    private data;
    constructor(container: HTMLElement, fills: string[], strokes: string[]);
    updateColors(fills: string[], strokes: string[]): void;
}
