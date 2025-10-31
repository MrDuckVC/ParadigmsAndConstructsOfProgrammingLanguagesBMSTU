"""
Контекстные менеджеры для измерения времени выполнения блока кода
"""

import time
from contextlib import contextmanager


# Реализация на основе класса
# Название класса обусловлено условием задачи
class cm_timer_1:  # pylint: disable=invalid-name

    def __enter__(self):
        self.start_time = time.time()
        return self

    def __exit__(self, exc_type, exc_val, exc_tb):
        elapsed_time = time.time() - self.start_time
        print(f"time: {elapsed_time:.5f}")


# Реализация с использованием contextlib
@contextmanager
def cm_timer_2():
    start_time = time.time()
    try:
        yield
    finally:
        elapsed_time = time.time() - start_time
        print(f"time: {elapsed_time:.5f}")


if __name__ == "__main__":
    # Тестирование
    print("Тест cm_timer_1:")
    with cm_timer_1():
        time.sleep(1.5)

    print("Тест cm_timer_2:")
    with cm_timer_2():
        time.sleep(0.5)
