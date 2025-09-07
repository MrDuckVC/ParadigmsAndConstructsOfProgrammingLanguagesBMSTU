#!/usr/bin/env python3
"""
Основной файл для тестирования классов геометрических фигур
"""
import sys
import os

# Импортируем внешний пакет
from colorama import Fore, Style, init

from lab_python_oop.rectangle import Rectangle
from lab_python_oop.circle import Circle
from lab_python_oop.square import Square

# Добавляем путь к пакету
sys.path.insert(0, os.path.join(os.path.dirname(__file__)))


def main():
    """
    Основная функция тестирования
    """
    # Инициализируем colorama
    init(autoreset=True)

    # Номер варианта
    n = 13

    print(Fore.CYAN + "=== Лабораторная работа №2 ===")
    print(Fore.CYAN + "=== Объектно-ориентированные возможности Python ===")
    print()

    # Создаем объекты фигур
    try:
        rectangle = Rectangle(n, n, "синий")
        circle = Circle(n, "зеленый")
        square = Square(n, "красный")

        # Выводим информацию о фигурах
        print(Fore.YELLOW + "Информация о геометрических фигурах:")
        print(Fore.GREEN + f"1. {rectangle}")
        print(Fore.GREEN + f"2. {circle}")
        print(Fore.GREEN + f"3. {square}")
        print()

        # Демонстрация работы внешнего пакета colorama
        print(Fore.MAGENTA + "=== Демонстрация внешнего пакета colorama ===")
        print(Fore.RED + "Этот текст красного цвета")
        print(Fore.BLUE + "Этот текст синего цвета")
        print(Fore.GREEN + "Этот текст зеленого цвета")
        print(Style.BRIGHT + Fore.WHITE + "Этот текст ярко-белый")
        print()

        # Дополнительная информация о фигурах
        print(Fore.CYAN + "=== Дополнительная информация ===")
        print(f"Тип прямоугольника: {type(rectangle).__name__}")
        print(f"Тип круга: {type(circle).__name__}")
        print(f"Тип квадрата: {type(square).__name__}")
        print()

        # Проверка наследования
        print(Fore.CYAN + "=== Проверка наследования ===")
        print(f"Квадрат является прямоугольником: {isinstance(square, Rectangle)}")
        print(f"Площадь квадрата: {square.area():.2f}")

    # Да, я жулик, но в данном коде сложно предположить возможные исключения
    except Exception as e:  # pylint: disable=broad-except
        print(Fore.RED + f"Ошибка: {e}")
        return 1

    return 0


if __name__ == "__main__":
    sys.exit(main())
