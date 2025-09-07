"""
Решение биквадратного уравнения
"""

from biquadratic import EquationSolverApp, procedural_main


def main():
    # Спрашиваем у пользователя какую реализацию использовать
    print("Какую реализацию использовать?")
    print("1. ООП")
    print("2. Процедурная")
    choice = input("Выберите 1 или 2: ")
    if choice == "1":
        EquationSolverApp().run()
    elif choice == "2":
        procedural_main()
    else:
        print("Неверный выбор. Попробуйте ещё раз.")

    # print("=== ООП вариант решения биквадратного уравнения ===")
    # oop_main()

    # print("\n=== Процедурный вариант решения биквадратного уравнения ===")
    # procedural_main()


if __name__ == "__main__":
    main()
