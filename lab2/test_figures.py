"""
Тесты для геометрических фигур
"""

import unittest
import sys
import os

from lab_python_oop.rectangle import Rectangle
from lab_python_oop.circle import Circle
from lab_python_oop.square import Square

# Добавляем путь к пакету
sys.path.insert(0, os.path.join(os.path.dirname(__file__)))


class TestFigures(unittest.TestCase):
    """Тесты для геометрических фигур"""

    def test_rectangle_area(self):
        """Тест площади прямоугольника"""
        rect = Rectangle(5, 10, "синий")
        self.assertEqual(rect.area(), 50)

    def test_circle_area(self):
        """Тест площади круга"""
        circle = Circle(5, "зеленый")
        self.assertAlmostEqual(circle.area(), 78.539816, places=5)

    def test_square_area(self):
        """Тест площади квадрата"""
        square = Square(5, "красный")
        self.assertEqual(square.area(), 25)

    def test_square_inheritance(self):
        """Тест наследования квадрата от прямоугольника"""
        square = Square(5, "красный")
        self.assertIsInstance(square, Rectangle)

    def test_color_property(self):
        """Тест свойства цвета"""
        rect = Rectangle(5, 10, "синий")
        self.assertEqual(rect.color.color, "синий")

        # Тест изменения цвета
        rect.color.color = "красный"
        self.assertEqual(rect.color.color, "красный")

    def test_figure_names(self):
        """Тест названий фигур"""
        rect = Rectangle(5, 10, "синий")
        circle = Circle(5, "зеленый")
        square = Square(5, "красный")

        self.assertEqual(rect.name, "Прямоугольник")
        self.assertEqual(circle.name, "Круг")
        self.assertEqual(square.name, "Квадрат")


if __name__ == "__main__":
    unittest.main()
