import matplotlib.pyplot as plot
import numpy as np
from task import Task
import matplotlib.dates as mdates
from helpers import if_string_convert_to_datetime
import matplotlib.ticker as ticker

# Script builds a matplotlib figure based on information, passed to it through json file.
# Path to the JSON must be passed in first command line argument.

class MatplotlibCS:
    def __init__(self, task):
        self.task = task

    def build_figure(self):
        t = Task(self.task)
        fig = plot.figure(figsize=[t.w/(1.0*t.dpi), t.h/(1.0*t.dpi)])

        # precreate, make basic settings and save subplots
        subplots = {}
        subplot_index = 1
        for i in range(0, t.rows):
            for j in range(0, t.columns):
                if (t.subplots[subplot_index - 1].is3d):
                    axes = fig.add_subplot(t.rows, t.columns, subplot_index, projection='3d')
                else:
                    axes = fig.add_subplot(t.rows, t.columns, subplot_index)
                subplots[subplot_index] = axes
                # self.set_grid(axes)
                subplot_index += 1

        # draw items on each subplot
        for subplot in t.subplots:
            axes = subplots[subplot.index]
            plot.sca(axes)
            self.set_titles(subplot, axes)
            for item in subplot.items:
                if item.is_visible == True:
                    item.plot(axes)
            if subplot.show_legend:
                plot.legend(loc=subplot.legend_location, frameon=subplot.frameon)

            self.set_grid(axes, subplot.grid)

        plot.tight_layout()

        self.save_figure_to_file(self.task)

        if not self.task["onlySaveImage"]:
            plot.show()

        plot.close("all")

    def save_figure_to_file(self, task):
        """
        Saves figure content to the file if it's path is provided

        :type task: dict
        :param task: Deserialized json with task description

        :return:
        """
        if "filename" in task and task["filename"] is not None:
            print ("Saving figure to file {0}".format(task["filename"]))
            plot.savefig(task["filename"], dpi=task["dpi"])

    def set_grid(self, axes, grid):
        """
        Setup axes grid
        :param axes:
        :return:
        """
        axes.grid(which=grid.which)
        axes.grid(which='minor', alpha=grid.minor_alpha)
        axes.grid(which='major', alpha=grid.major_alpha)
        axes.grid(grid.on)

        if grid.x_lim is not None:
            axes.set_xlim(grid.x_lim[0], grid.x_lim[1])

        if grid.y_lim is not None:
            axes.set_ylim(grid.y_lim[0], grid.y_lim[1])

        if grid.z_lim is not None:
            axes.set_zlim(grid.z_lim[0], grid.z_lim[1])

        # if time ticks defined
        if grid.x_time_ticks is not None and len(grid.x_time_ticks) > 0:
            timeTicks = []
            for stringTick in grid.x_time_ticks:
                timeTick = if_string_convert_to_datetime(stringTick)
                timeTicks.append(timeTick)

            # distance between nodes will be equal
            if grid.regular_time_axis:
                n = len(grid.x_time_ticks)

                def format_date(x, pos=None):
                    thisind = np.clip(int(x + 0.5), 0, n - 1)
                    return timeTicks[thisind].strftime(grid.time_ticks_format['value'])

                axes.xaxis.set_major_formatter(ticker.FuncFormatter(format_date))

            # else distance between time nodes will be as is
            else:
                formatter = mdates.DateFormatter(grid.time_ticks_format['value'])
                axes.xaxis.set_major_formatter(formatter)
                axes.set_xticks(timeTicks)

        # if no time ticks defined
        elif grid.x_major_ticks is not None:
            axes.set_xticks(self.build_ticks_list(grid.x_major_ticks))
            if grid.x_minor_ticks is not None:
                axes.set_xticks(self.build_ticks_list(grid.x_minor_ticks), minor=True)

        if grid.y_major_ticks is not None:
            axes.set_yticks(self.build_ticks_list(grid.y_major_ticks))

        if grid.y_minor_ticks is not None:
            axes.set_yticks(self.build_ticks_list(grid.y_minor_ticks), minor=True)

        if grid.z_major_ticks is not None:
            axes.set_zticks(self.build_ticks_list(grid.z_major_ticks))

        if grid.z_minor_ticks is not None:
            axes.set_zticks(self.build_ticks_list(grid.z_minor_ticks), minor=True)

        # set font and rotation
        labels = axes.get_xticklabels()

        if grid.x_tick_fontsize is not None and grid.x_tick_fontsize!=0:
            plot.setp(labels, fontsize=grid.x_tick_fontsize)

        if grid.x_tick_rotation is not None and grid.x_tick_rotation!=0:
            plot.setp(labels, rotation=grid.x_tick_rotation)

    def build_ticks_list(self, lims):
        '''
        First two values in lims are the minimum and the maximum values, while third value is a step.
        Values starting from 4 are additional values which will be added to the grid
        :param lims:
        :return:
        '''
        ticks = np.arange(lims[0], lims[1] + lims[2], lims[2])
        if len(lims) > 3:
            for i in range(3, len(lims)):
                ticks = np.append(ticks, [lims[i]])
        return ticks

    def set_titles(self, task, axes):
        """
        Setup subplot X and Y axis titles

        :param task:
        :return:
        """
        axes.set_title(u"{0}".format(task.title))
        axes.set_xlabel(task.xtitle)
        axes.set_ylabel(task.ytitle)
        if (task.ztitle != None):
            axes.set_zlabel(task.ztitle)
