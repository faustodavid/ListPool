using AutoFixture;
using Xunit;

namespace ListPool.UnitTests
{
    public abstract class ListPoolTestsBase
    {
        protected static readonly Fixture s_fixture = new Fixture();

        [Fact]
        public abstract void Add_item_without_indicate_capacity_of_list();

        [Fact]
        public abstract void Add_items_when_capacity_is_full_then_buffer_autogrow();

        [Fact]
        public abstract void Contains_empty_ListPool_without_indicating_capacity_returns_false();

        [Fact]
        public abstract void Contains_return_true_when_item_exists();

        [Fact]
        public abstract void CopyTo_copy_all_elements_to_target_array();

        [Fact]
        public abstract void Count_property_is_for_items_Added_count_not_capacity_of_buffer();

        [Fact]
        public abstract void Create_list_and_add_values();

        [Fact]
        public abstract void Create_list_and_add_values_after_clear();

        [Fact]
        public abstract void Create_without_parameters_should_add_and_get_items();

        [Fact]
        public abstract void Enumerate_when_capacity_is_not_set_dont_throw_exception();

        [Fact]
        public abstract void Get_item_with_index_above_itemsCount_throws_ArgumentOutOfRangeException();

        [Fact]
        public abstract void Get_item_with_index_bellow_zero_throws_ArgumentOutOfRangeException();

        [Fact]
        public abstract void IndexOf_empty_ListPool_without_indicating_capacity_returns_negative_one();

        [Fact]
        public abstract void IndexOf_returns_index_of_item();

        [Fact]
        public abstract void Insert_at_existing_index_move_items_up();

        [Fact]
        public abstract void Insert_at_the_end_add_new_item();

        [Fact]
        public abstract void Insert_item_with_index_above_itemsCount_throws_ArgumentOutOfRangeException();

        [Fact]
        public abstract void Insert_item_with_index_bellow_zero_throws_ArgumentOutOfRangeException();

        [Fact]
        public abstract void Insert_items_when_capacity_is_full_then_buffer_autogrow();

        [Fact]
        public abstract void Insert_without_indicating_capacity_of_list();

        [Fact]
        public abstract void Readonly_property_is_always_false();

        [Fact]
        public abstract void Remove_item_that_doesnt_exists_return_false();

        [Fact]
        public abstract void Remove_when_item_exists_remove_item_and_decrease_itemsCount();

        [Fact]
        public abstract void Remove_when_item_is_null_return_false();

        [Fact]
        public abstract void RemoveAt_when_item_exists_remove_item_and_decrease_itemsCount();

        [Fact]
        public abstract void RemoveAt_with_index_above_itemsCount_throws_ArgumentOutOfRangeException();

        [Fact]
        public abstract void RemoveAt_with_index_bellow_zero_throws_ArgumentOutOfRangeException();

        [Fact]
        public abstract void RemoveAt_with_index_zero_when_not_item_added_throws_ArgumentOutOfRangeException();

        [Fact]
        public abstract void Set_at_existing_index_update_item();

        [Fact]
        public abstract void Set_item_with_index_above_itemsCount_throws_ArgumentOutOfRangeException();

        [Fact]
        public abstract void Set_item_with_index_bellow_zero_throws_ArgumentOutOfRangeException();
    }
}
