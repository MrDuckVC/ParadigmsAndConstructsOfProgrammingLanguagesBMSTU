"""
Сортировка списка по абсолютному значению в обратном порядке
"""

data = [4, -30, 30, 100, -100, 123, 1, 0, -1, -4]

if __name__ == "__main__":
    # Без lambda
    result = sorted(data, key=abs, reverse=True)
    print("Без lambda:", result)

    # С lambda
    # Использование lambda здесь избыточно, но это требуется по условию задания
    result_with_lambda = sorted(data, key=lambda x: abs(x), reverse=True)  # pylint: disable=unnecessary-lambda
    print("С lambda:", result_with_lambda)
