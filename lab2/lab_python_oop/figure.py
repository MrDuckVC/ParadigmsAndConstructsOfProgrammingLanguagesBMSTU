"""Модуль с абстрактным классом Геометрическая фигура"""

from abc import ABC, abstractmethod


class Figure(ABC):
    """Абстрактный класс Геометрическая фигура"""

    @abstractmethod
    def area(self):
        """Абстрактный метод для вычисления площади фигуры"""
        pass

    @property
    @abstractmethod
    def name(self):
        """Абстрактное свойство для получения названия фигуры"""
        pass

    def __repr__(self):
        # f-строки быстрее, но по заданию нужно использовать format
        return "{} {} цвета площадью {:.2f}".format(  # pylint: disable=consider-using-f-string
            self.name, self.color.color, self.area()
        )
