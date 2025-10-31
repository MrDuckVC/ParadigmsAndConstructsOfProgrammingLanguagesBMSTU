"""Модуль с классом Круг"""

import math
from typing import Union

from .figure import Figure
from .color import Color


class Circle(Figure):
    """Класс Круг"""

    FIGURE_NAME: str = "Круг"

    def __init__(self, radius: Union[int, float], color: str):
        self.radius = radius
        self.color: Color = Color(color)

    def area(self) -> float:
        """Вычисление площади круга"""
        return math.pi * self.radius**2

    @property
    def name(self) -> str:
        """Название фигуры"""
        return self.FIGURE_NAME

    def __repr__(self):
        # f-строки быстрее, но по заданию нужно использовать format
        return "{} {} цвета радиусом {}. Площадь: {:.2f}".format(  # pylint: disable=consider-using-f-string
            self.name, self.color.color, self.radius, self.area()
        )
