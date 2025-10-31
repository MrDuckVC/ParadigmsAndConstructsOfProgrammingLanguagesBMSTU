"""Модуль с классом Прямоугольник"""

from typing import Union

from .figure import Figure
from .color import Color


class Rectangle(Figure):
    """Класс Прямоугольник"""

    FIGURE_NAME: str = "Прямоугольник"

    def __init__(self, width: Union[int, float], height: Union[int, float], color: str):
        self.width = width
        self.height = height
        self.color: Color = Color(color)

    def area(self) -> Union[int, float]:
        """Вычисление площади прямоугольника"""
        return self.width * self.height

    @property
    def name(self) -> str:
        """Название фигуры"""
        return self.FIGURE_NAME

    def __repr__(self):
        # f-строки быстрее, но по заданию нужно использовать format
        return "{} {} цвета шириной {} и высотой {}. Площадь: {:.2f}".format(  # pylint: disable=consider-using-f-string
            self.name, self.color.color, self.width, self.height, self.area()
        )
