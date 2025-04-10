import type { ChartType } from 'ag-grid-community';
import type { ThemeTemplateParameters } from '../../miniChartsContainer';
import { MiniChart } from '../miniChart';
export declare class MiniTreemap extends MiniChart {
    static chartType: ChartType;
    private readonly rects;
    constructor(container: HTMLElement, fills: string[], strokes: string[], themeTemplate: ThemeTemplateParameters, isCustomTheme: boolean);
    updateColors(fills: string[], strokes: string[], themeTemplate?: ThemeTemplateParameters, isCustomTheme?: boolean): void;
}
