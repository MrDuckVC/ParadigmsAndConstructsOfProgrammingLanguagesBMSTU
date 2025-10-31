"""Модуль с классом Цвет фигуры"""


class Color:
    """Класс Цвет фигуры"""

    def __init__(self, color: str):
        self._color: str = color

    @property
    def color(self) -> str:
        """Свойство для получения цвета"""
        return self._color

    @color.setter
    def color(self, value: str):
        """
        Свойство для установки цвета

        Args:
            value (str): Значение цвета
        """
        self._color = value
