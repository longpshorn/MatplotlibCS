import matplotlib.pyplot as plot

class Scatter:
    def __init__(self, jsonDict):
        self.__dict__ = jsonDict

    def plot(self, axes):
        linewidth = self.lineWidth[0] if len(self.lineWidth) == 1 else self.lineWidth
        colors = self.color[0]["value"] if len(self.color) == 1 else map(lambda x: x["value"], self.color)
        axes.scatter(
            self.x,
            self.y,
            self.z,
            c=colors,
            marker=self.marker,
            alpha=self.alpha,
            linewidths=linewidth,
            plotnonfinite=self.plotnonfinite
        )
