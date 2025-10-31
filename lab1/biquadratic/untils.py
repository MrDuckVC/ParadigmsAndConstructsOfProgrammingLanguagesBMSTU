"""
Вспомогательные функции
"""

import argparse
import math

INFINITY_ROOTS_TEXT = "Бесконечное множество решений"


def create_biquadratic_equation_string(a: float, b: float, c: float) -> str:
    """
    Создание строки для квадратного уравнения

    Args:
        a (float): Коэффициент A
        b (float): Коэффициент B
        c (float): Коэффициент C

    Returns:
        str: Строка для квадратного уравнения
    """

    def int_or_float(x: float) -> int | float:
        return int(x) if x.is_integer() else x

    def get_sign(x: float) -> str:
        return "+" if x >= 0 else "-"

    return f"{int_or_float(a)}x^4 {get_sign(b)} {abs(int_or_float(b))}x^2 {get_sign(c)} {abs(int_or_float(c))} = 0"


def parse_args() -> argparse.Namespace:
    """
    Парсинг аргументов командной строки

    Returns:
        argparse.Namespace: Объект с аргументами
    """
    parser = argparse.ArgumentParser(
        description="Решение биквадратного уравнения (процедурный вариант)"
    )
    parser.add_argument("a", nargs="?", type=float, help="Коэффициент A")
    parser.add_argument("b", nargs="?", type=float, help="Коэффициент B")
    parser.add_argument("c", nargs="?", type=float, help="Коэффициент C")
    return parser.parse_args()


def get_coef(prompt: str) -> float:
    """
    Получение коэффициента

    Args:
        prompt (str): Просим пользователя ввести коэффициент

    Returns:
        float: Коэффициент
    """
    while True:
        try:
            return float(input(prompt))
        except ValueError:
            print("Некорректный ввод. Попробуйте снова.")


def print_roots(roots: list[float] | list[str]):
    """
    Вывод корней

    Args:
        roots (list[float] | list[str]): Список корней или текст об бесконечном количестве корней
    """
    if not roots:
        print("Действительных корней нет")
    elif roots == [INFINITY_ROOTS_TEXT]:
        print(INFINITY_ROOTS_TEXT)
    else:
        print(f"Найдено {len(roots)} действительных корней:")
        for i, r in enumerate(roots, 1):
            print(f"  x{i} = {r:.4f}")


def solve_biquadratic(a: float, b: float, c: float) -> list[float] | list[str]:
    """
    Решение биквадратного уравнения

    Args:
        a (float): Коэффициент A
        b (float): Коэффициент B
        c (float): Коэффициент C

    Returns:
        list[float] | list[str]: Список корней или текст об бесконечном количестве корней
    """
    if a == 0 and b == 0 and c == 0:
        return [INFINITY_ROOTS_TEXT]

    if a == 0:
        if b == 0:
            return []
        y = -c / b
        if y < 0:
            return []
        return sorted({math.sqrt(y), -math.sqrt(y)})

    d = b * b - 4 * a * c
    if d < 0:
        return []

    roots = []
    for y in [(-b + math.sqrt(d)) / (2 * a), (-b - math.sqrt(d)) / (2 * a)]:
        if y >= 0:
            roots.extend([math.sqrt(y), -math.sqrt(y)])
    return sorted(set(roots))
