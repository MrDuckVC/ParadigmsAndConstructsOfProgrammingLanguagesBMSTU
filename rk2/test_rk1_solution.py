"""
Модульные тесты для Рубежного контроля №1.
"""

import unittest
from rk1_solution import (
    House,
    Street,
    HouseStreet,
    solve_b1,
    solve_b2,
    solve_b3,
)


class TestRK1Queries(unittest.TestCase):
    """Тест-кейс для 3-х запросов варианта Б."""

    def setUp(self):
        """
        Настройка тестового окружения (Fixtures).

        Создает наборы тестовых данных (улицы, дома, связи) и подготавливает
        входные данные ('one_to_many', 'many_to_many') для каждого тестового метода.
        """
        self.streets: list[Street] = [
            Street(1, "ул. Ленина"),
            Street(2, "ул. Пушкина"),
            Street(3, "просп. Мира"),
        ]
        self.houses: list[House] = [
            House(1, "10а", 50, 1),
            House(2, "12", 120, 1),
            House(3, "5а", 80, 2),
            House(4, "7б", 200, 3),
            House(5, "22", 75, 2),
        ]
        self.houses_streets: list[HouseStreet] = [
            HouseStreet(1, 1),
            HouseStreet(2, 1),
            HouseStreet(1, 2),
            HouseStreet(2, 3),
            HouseStreet(3, 4),
            HouseStreet(2, 5),
        ]

        self.one_to_many = [
            (h.number, h.population, s.name)
            for s in self.streets
            for h in self.houses
            if h.street_id == s.identifier
        ]

        many_to_many_temp = [
            (s.name, hs.street_id, hs.house_id)
            for s in self.streets
            for hs in self.houses_streets
            if s.identifier == hs.street_id
        ]
        self.many_to_many = [
            (h.number, h.population, s_name)
            for s_name, s_id, h_id in many_to_many_temp
            for h in self.houses
            if h.identifier == h_id
        ]

    def test_solve_b1(self):
        """Тестирование Задачи Б1 (сортировка домов по номеру).

        Проверяет, что функция solve_b1 корректно сортирует список 1:M по номеру дома.
        """
        expected = [
            ("10а", 50, "ул. Ленина"),
            ("12", 120, "ул. Ленина"),
            ("22", 75, "ул. Пушкина"),
            ("5а", 80, "ул. Пушкина"),
            ("7б", 200, "просп. Мира"),
        ]

        result = solve_b1(self.one_to_many)

        self.assertEqual(result, expected)

    def test_solve_b2(self):
        """Тестирование Задачи Б2 (количество домов по улицам).

        Проверяет, что функция solve_b2 корректно группирует дома по улицам,
        подсчитывает их количество и сортирует результат по количеству домов.
        """
        expected = [
            ("просп. Мира", 1),
            ("ул. Ленина", 2),
            ("ул. Пушкина", 2),
        ]

        result = solve_b2(self.streets, self.one_to_many)

        self.assertEqual(result, expected)

    def test_solve_b3(self):
        """Тестирование Задачи Б3 (дома на 'а' в M:M).

        Проверяет, что функция solve_b3 корректно фильтрует список M:M,
        оставляя только дома с номером, оканчивающимся на 'а'.
        """
        expected = [
            ("10а", "ул. Ленина"),
            ("10а", "ул. Пушкина"),
            ("5а", "ул. Пушкина"),
        ]

        result = solve_b3(self.many_to_many)

        self.assertEqual(result, expected)


if __name__ == "__main__":
    unittest.main()
