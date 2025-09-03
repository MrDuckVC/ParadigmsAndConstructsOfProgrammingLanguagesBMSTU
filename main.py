"""
Решение биквадратного уравнения ax^4 + bx^2 + c = 0

Usage:
    python main.py <a> <b> <c>

Example:
    python main.py 1 2 3
    python main.py 2 -3 1
    python main.py 0 0 0
"""

import sys
import math


INFINITY_ROOTS_TEXT = "Бесконечное множество решений"


def get_coef(index, prompt):
    """
    Читаем коэффициент из командной строки или вводим с клавиатуры

    Args:
        index (int): Номер параметра в командной строке
        prompt (str): Приглашение для ввода коэффициента

    Returns:
        float: Коэффициент биквадратного уравнения
    """
    while True:
        try:
            # Пробуем прочитать коэффициент из командной строки
            if len(sys.argv) > index:
                coef_str = sys.argv[index]
                coef = float(coef_str)
                return coef
            else:
                # Вводим с клавиатуры
                print(prompt)
                coef_str = input()
                coef = float(coef_str)
                return coef
        except (ValueError, IndexError):
            # Если коэффициент некорректный, игнорируем и запрашиваем повторно
            if len(sys.argv) > index:
                # Если был передан некорректный параметр, игнорируем его
                sys.argv = sys.argv[:index] + sys.argv[index + 1 :]
            print("Некорректный ввод. Пожалуйста, введите действительное число.")


def solve_biquadratic(a, b, c):
    """
    Решение биквадратного уравнения ax^4 + bx^2 + c = 0

    Args:
        a (float): коэффициент A
        b (float): коэффициент B
        c (float): коэффициент C

    Returns:
        list[float]: Список действительных корней
    """
    result = []

    # Проверка особых случаев
    if a == 0:
        # Уравнение становится квадратным bx^2 + c = 0
        if b == 0:
            if c == 0:
                # Бесконечное множество решений
                return [INFINITY_ROOTS_TEXT]
            else:
                # Нет решений
                return []
        else:
            # Решаем bx^2 + c = 0
            if -c / b >= 0:
                root = math.sqrt(-c / b)
                result.append(root)
                result.append(-root)
            return result

    # Решаем квадратное уравнение относительно y = x^2
    discriminant = b * b - 4 * a * c

    if discriminant < 0:
        # Нет действительных корней
        return result

    # Вычисляем корни для y
    y1 = (-b + math.sqrt(discriminant)) / (2 * a)
    y2 = (-b - math.sqrt(discriminant)) / (2 * a)

    # Находим x из y = x^2
    if y1 >= 0:
        root1 = math.sqrt(y1)
        root2 = -math.sqrt(y1)
        result.append(root1)
        result.append(root2)

    if y2 >= 0 and y2 != y1:  # Чтобы избежать дублирования корней
        root3 = math.sqrt(y2)
        root4 = -math.sqrt(y2)
        result.append(root3)
        result.append(root4)

    # Убираем дубликаты и сортируем
    result = sorted(list(set(result)))
    return result


def main():
    """
    Основная функция
    """
    print("Решение биквадратного уравнения ax^4 + bx^2 + c = 0")

    a = get_coef(1, "Введите коэффициент A:")
    b = get_coef(2, "Введите коэффициент B:")
    c = get_coef(3, "Введите коэффициент C:")

    print(f"\nУравнение: {a}x^4 + {b}x^2 + {c} = 0")

    # Вычисление корней
    roots = solve_biquadratic(a, b, c)

    # Вывод корней
    if not roots:
        print("Действительных корней нет")
    elif roots == [INFINITY_ROOTS_TEXT]:
        print(INFINITY_ROOTS_TEXT)
    else:
        print(f"Найдено {len(roots)} действительных корней:")
        for i, root in enumerate(roots, 1):
            print(f"Корень {i}: x = {root:.4f}")


# Если сценарий запущен из командной строки
if __name__ == "__main__":
    main()
