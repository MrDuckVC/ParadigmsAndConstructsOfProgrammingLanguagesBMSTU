"""
Модуль с классом Квадрат
"""

from typing import Union
from .rectangle import Rectangle


class Square(Rectangle):
    """
    Класс Квадрат (наследуется от Прямоугольника)
    """

    FIGURE_NAME: str = "Квадрат"

    def __init__(self, side: Union[int, float], color: str):
        # Вызываем конструктор родительского класса
        super().__init__(side, side, color)

    @property
    def name(self) -> str:
        """Название фигуры"""
        return self.FIGURE_NAME

    def __repr__(self):
        # f-строки быстрее, но по заданию нужно использовать format
        return "{} {} цвета со стороной {}. Площадь: {:.2f}".format(  # pylint: disable=consider-using-f-string
            self.name, self.color.color, self.width, self.area()
        )
