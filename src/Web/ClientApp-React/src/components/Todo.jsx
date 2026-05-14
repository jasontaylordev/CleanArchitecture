import { useState, useEffect, useRef } from 'react';
import { Plus, Settings, MoreHorizontal } from 'lucide-react';
import { TodoListsClient, TodoItemsClient } from '../web-api-client.ts';

const listsClient = new TodoListsClient();
const itemsClient = new TodoItemsClient();

export function Tasks() {
  const [lists, setLists] = useState(null);
  const [priorityLevels, setPriorityLevels] = useState([]);
  const [colours, setColours] = useState([]);
  const [selectedListId, setSelectedListId] = useState(null);
  const [selectedItem, setSelectedItem] = useState(null);
  const [editingItem, setEditingItem] = useState(null);
  const [editValue, setEditValue] = useState('');
  const [newItemTitle, setNewItemTitle] = useState('');
  const [addingItem, setAddingItem] = useState(false);
  const [listOptionsEditor, setListOptionsEditor] = useState({});
  const [itemDetailsEditor, setItemDetailsEditor] = useState({});
  const [newListTitle, setNewListTitle] = useState('');
  const [newListColour, setNewListColour] = useState('');
  const [newListError, setNewListError] = useState('');

  const originalTitle = useRef('');
  const editCancelledRef = useRef(false);
  const newItemCancelledRef = useRef(false);

  const newListDialogRef = useRef(null);
  const listOptionsDialogRef = useRef(null);
  const deleteListDialogRef = useRef(null);
  const itemDetailsDialogRef = useRef(null);

  useEffect(() => {
    listsClient.getTodoLists().then(result => {
      setLists(result.lists);
      setPriorityLevels(result.priorityLevels);
      const apiColours = result.colours.map(c => ({ name: c.name, code: c.code }));
      setColours(apiColours);
      setNewListColour(apiColours[0]?.code ?? '');
      if (result.lists.length) setSelectedListId(result.lists[0].id);
    }).catch(console.error);
  }, []);

  useEffect(() => {
    setNewItemTitle('');
    setAddingItem(false);
  }, [selectedListId]);

  const selectedList = lists?.find(l => l.id === selectedListId) ?? null;
  const remainingItems = list => list.items.filter(t => !t.done).length;

  // ── Lists ──────────────────────────────────────────────────────────────────

  const showNewListDialog = () => {
    setNewListTitle('');
    setNewListColour(colours[0].code);
    setNewListError('');
    newListDialogRef.current.showModal();
    setTimeout(() => document.getElementById('newListTitle')?.focus(), 50);
  };

  const closeNewListDialog = () => {
    newListDialogRef.current.close();
    setNewListTitle('');
    setNewListColour(colours[0].code);
    setNewListError('');
  };

  const commitNewList = async () => {
    if (!newListTitle.trim()) return;
    try {
      const id = await listsClient.createTodoList({ title: newListTitle.trim(), colour: newListColour });
      const newList = { id, title: newListTitle.trim(), colour: newListColour, items: [] };
      setLists(ls => [...ls, newList]);
      setSelectedListId(id);
      closeNewListDialog();
    } catch (e) {
      try {
        const errors = JSON.parse(e.response).errors;
        if (errors?.Title) { setNewListError(errors.Title[0]); return; }
      } catch { /* ignore */ }
      setNewListError('Failed to create list.');
    }
  };

  const showListOptionsDialog = () => {
    setListOptionsEditor({ id: selectedList.id, title: selectedList.title, colour: selectedList.colour || colours[0].code });
    listOptionsDialogRef.current.showModal();
  };

  const closeListOptionsDialog = () => {
    listOptionsDialogRef.current.close();
    setListOptionsEditor({});
  };

  const updateListOptions = async () => {
    try {
      await listsClient.updateTodoList(selectedList.id, listOptionsEditor);
      setLists(ls => ls.map(l => l.id === selectedList.id ? { ...l, title: listOptionsEditor.title, colour: listOptionsEditor.colour } : l));
      closeListOptionsDialog();
    } catch (e) { console.error(e); }
  };

  const confirmDeleteList = () => {
    closeListOptionsDialog();
    deleteListDialogRef.current.showModal();
  };

  const closeDeleteListDialog = () => deleteListDialogRef.current.close();

  const deleteListConfirmed = async () => {
    try {
      await listsClient.deleteTodoList(selectedList.id);
      const remaining = lists.filter(l => l.id !== selectedList.id);
      setLists(remaining);
      setSelectedListId(remaining.length ? remaining[0].id : null);
      closeDeleteListDialog();
    } catch (e) { console.error(e); }
  };

  // ── Items ──────────────────────────────────────────────────────────────────

  const showItemDetailsDialog = (item) => {
    setSelectedItem(item);
    setItemDetailsEditor({ ...item });
    itemDetailsDialogRef.current.showModal();
  };

  const closeItemDetailsDialog = () => {
    itemDetailsDialogRef.current.close();
    setSelectedItem(null);
    setItemDetailsEditor({});
  };

  const updateItemDetails = async () => {
    const isMoving = selectedItem.listId !== itemDetailsEditor.listId;
    try {
      await itemsClient.updateTodoItemDetail(selectedItem.id, itemDetailsEditor);
      setLists(ls => ls.map(l => {
        if (l.id === selectedItem.listId && isMoving)
          return { ...l, items: l.items.filter(i => i.id !== selectedItem.id) };
        if (l.id === itemDetailsEditor.listId && isMoving)
          return { ...l, items: [...l.items, { ...selectedItem, ...itemDetailsEditor }] };
        if (l.id === selectedItem.listId)
          return { ...l, items: l.items.map(i => i.id === selectedItem.id ? { ...i, priority: itemDetailsEditor.priority, note: itemDetailsEditor.note } : i) };
        return l;
      }));
      closeItemDetailsDialog();
    } catch (e) { console.error(e); }
  };

  const deleteItem = async (item) => {
    if (itemDetailsDialogRef.current?.open) closeItemDetailsDialog();
    try {
      await itemsClient.deleteTodoItem(item.id);
      setLists(ls => ls.map(l => l.id === selectedListId
        ? { ...l, items: l.items.filter(i => i.id !== item.id) } : l));
    } catch (e) { console.error(e); }
  };

  const updateCheckbox = async (item, done) => {
    const updated = { ...item, done };
    setLists(ls => ls.map(l => l.id === selectedListId
      ? { ...l, items: l.items.map(i => i.id === item.id ? updated : i) } : l));
    try { await itemsClient.updateTodoItem(item.id, updated); }
    catch (e) { console.error(e); }
  };

  const editItem = (item, inputId) => {
    originalTitle.current = item.title;
    setEditValue(item.title);
    setEditingItem(item);
    setTimeout(() => document.getElementById(inputId)?.focus(), 100);
  };

  const cancelEdit = (e) => {
    editCancelledRef.current = true;
    setLists(ls => ls.map(l => ({
      ...l, items: l.items.map(i => i === editingItem ? { ...i, title: originalTitle.current } : i)
    })));
    setEditingItem(null);
    e?.target.blur();
  };

  const commitEdit = async () => {
    if (!editValue.trim()) {
      await deleteItem(editingItem);
      setEditingItem(null);
      return;
    }
    const updated = { ...editingItem, title: editValue.trim() };
    setLists(ls => ls.map(l => l.id === selectedListId
      ? { ...l, items: l.items.map(i => i === editingItem ? updated : i) } : l));
    setEditingItem(null);
    try { await itemsClient.updateTodoItem(updated.id, updated); }
    catch (e) { console.error(e); }
  };

  const startAddingItem = () => {
    setAddingItem(true);
    setTimeout(() => document.getElementById('newItemInput')?.focus(), 50);
  };

  const cancelNewItem = (e) => {
    newItemCancelledRef.current = true;
    setAddingItem(false);
    setNewItemTitle('');
    e?.target.blur();
  };

  const commitNewItem = async () => {
    setAddingItem(false);
    if (!newItemTitle.trim()) { setNewItemTitle(''); return; }
    const title = newItemTitle.trim();
    const listId = selectedListId;
    setNewItemTitle('');
    try {
      const id = await itemsClient.createTodoItem({ title, listId });
      setLists(ls => ls.map(l => l.id === listId
        ? { ...l, items: [...l.items, { id, listId, title, done: false, priority: priorityLevels[0]?.id }] } : l));
    } catch (e) { console.error(e); }
  };

  if (!lists) return <span aria-busy="true">Loading&hellip;</span>;

  return (
    <>
      <hgroup>
        <h1>Tasks</h1>
        <p>Manage your todo lists and tasks.</p>
      </hgroup>

      <div className="todo-layout">

        {/* Sidebar */}
        <div className="todo-sidebar">
          <div className="todo-panel-header">
            <h2>Lists</h2>
            <button className="icon-btn" onClick={showNewListDialog}><Plus size={20} strokeWidth={2} /></button>
          </div>
          <ul>
            {lists.map(list => (
              <li key={list.id}
                aria-current={selectedList === list ? 'true' : undefined}
                onClick={() => setSelectedListId(list.id)}>
                <span className="colour-dot" style={{ background: list.colour }} aria-hidden="true"></span>
                <span>{list.title}</span>
                <small>{remainingItems(list)}</small>
              </li>
            ))}
          </ul>
        </div>

        {/* Items panel */}
        {selectedList && (
          <div className="todo-main">
            <div className="todo-panel-header">
              <h2 style={{ color: selectedList.colour }}>{selectedList.title}</h2>
              <button className="icon-btn" onClick={showListOptionsDialog}><Settings size={20} strokeWidth={2} /></button>
            </div>

            {selectedList.items.map((item, i) => (
              <div key={item.id} className="todo-item">
                <input type="checkbox" checked={item.done}
                  onChange={e => updateCheckbox(item, e.target.checked)} />
                {editingItem === item ? (
                  <input id={`itemTitle${i}`} type="text" className="todo-item-input"
                    value={editValue}
                    onChange={e => setEditValue(e.target.value)}
                    onKeyDown={e => {
                      if (e.key === 'Enter') { e.target.blur(); }
                      if (e.key === 'Escape') cancelEdit(e);
                    }}
                    onBlur={() => {
                      if (editCancelledRef.current) { editCancelledRef.current = false; return; }
                      commitEdit();
                    }}
                    autoFocus maxLength={200} />
                ) : (
                  <span className={`todo-item-text${item.done ? ' todo-done' : ''}`}
                    onClick={() => editItem(item, `itemTitle${i}`)}>
                    {item.title}
                  </span>
                )}
                {item.id !== 0 && (
                  <button className="icon-btn" onClick={() => showItemDetailsDialog(item)}><MoreHorizontal size={20} strokeWidth={2} /></button>
                )}
              </div>
            ))}

            <div className="todo-item todo-new-item">
              <input type="checkbox" disabled />
              {addingItem ? (
                <input id="newItemInput" type="text" className="todo-item-input"
                  value={newItemTitle}
                  onChange={e => setNewItemTitle(e.target.value)}
                  onKeyDown={e => {
                    if (e.key === 'Enter') commitNewItem();
                    if (e.key === 'Escape') cancelNewItem(e);
                  }}
                  onBlur={() => {
                    if (newItemCancelledRef.current) { newItemCancelledRef.current = false; return; }
                    commitNewItem();
                  }}
                  maxLength={200} />
              ) : (
                <span className="todo-item-text todo-new-item-placeholder"
                  onClick={startAddingItem}>New task…</span>
              )}
            </div>
          </div>
        )}
      </div>

      {/* New List dialog */}
      <dialog ref={newListDialogRef}>
        <article>
          <header>
            <h3>New List</h3>
            <button rel="prev" aria-label="Close" onClick={closeNewListDialog}></button>
          </header>
          <label htmlFor="newListTitle">Title</label>
          <input type="text" id="newListTitle" placeholder="List title…"
            value={newListTitle} onChange={e => setNewListTitle(e.target.value)}
            aria-invalid={newListError ? 'true' : undefined}
            onKeyDown={e => e.key === 'Enter' && commitNewList()}
            maxLength={200} />
          {newListError && <small>{newListError}</small>}
          <label>Colour</label>
          <div className="colour-picker">
            {colours.map(c => (
              <button key={c.code} type="button" className={`colour-swatch${newListColour === c.code ? ' selected' : ''}`}
                style={{ background: c.code }} aria-label={c.name}
                onClick={() => setNewListColour(c.code)} />
            ))}
          </div>
          <footer>
            <button className="secondary" onClick={closeNewListDialog}>Cancel</button>
            <button onClick={commitNewList}>Create</button>
          </footer>
        </article>
      </dialog>

      {/* List Options dialog */}
      <dialog ref={listOptionsDialogRef}>
        <article>
          <header>
            <h3>List Options</h3>
            <button rel="prev" aria-label="Close" onClick={closeListOptionsDialog}></button>
          </header>
          <label htmlFor="listOptionsTitle">Title</label>
          <input type="text" id="listOptionsTitle" placeholder="List name…"
            value={listOptionsEditor.title || ''}
            onChange={e => setListOptionsEditor(ed => ({ ...ed, title: e.target.value }))}
            onKeyDown={e => e.key === 'Enter' && updateListOptions()}
            maxLength={200} />
          <label>Colour</label>
          <div className="colour-picker">
            {colours.map(c => (
              <button key={c.code} type="button" className={`colour-swatch${listOptionsEditor.colour === c.code ? ' selected' : ''}`}
                style={{ background: c.code }} aria-label={c.name}
                onClick={() => setListOptionsEditor(ed => ({ ...ed, colour: c.code }))} />
            ))}
          </div>
          <footer>
            <button className="danger" style={{ marginInlineEnd: 'auto' }} onClick={confirmDeleteList}>Delete</button>
            <button className="secondary" onClick={closeListOptionsDialog}>Cancel</button>
            <button onClick={updateListOptions}>Update</button>
          </footer>
        </article>
      </dialog>

      {/* Delete List dialog */}
      <dialog ref={deleteListDialogRef}>
        <article>
          <header>
            <h3>Delete "{selectedList?.title}"?</h3>
            <button rel="prev" aria-label="Close" onClick={closeDeleteListDialog}></button>
          </header>
          <p>All items will be permanently deleted.</p>
          <footer>
            <button className="secondary" onClick={closeDeleteListDialog}>Cancel</button>
            <button className="danger"
              style={{ '--pico-background-color': 'var(--pico-del-color)', '--pico-border-color': 'var(--pico-del-color)', '--pico-color': '#fff' }}
              onClick={deleteListConfirmed}>Delete</button>
          </footer>
        </article>
      </dialog>

      {/* Item Details dialog */}
      <dialog ref={itemDetailsDialogRef}>
        <article>
          <header>
            <h3>Item Details</h3>
            <button rel="prev" aria-label="Close" onClick={closeItemDetailsDialog}></button>
          </header>
          <label htmlFor="itemList">List</label>
          <select id="itemList" value={itemDetailsEditor.listId || ''}
            onChange={e => setItemDetailsEditor(ed => ({ ...ed, listId: +e.target.value }))}>
            {lists.map(list => (
              <option key={list.id} value={list.id}>{list.title}</option>
            ))}
          </select>
          <label htmlFor="itemPriority">Priority</label>
          <select id="itemPriority" value={itemDetailsEditor.priority || ''}
            onChange={e => setItemDetailsEditor(ed => ({ ...ed, priority: +e.target.value }))}>
            {priorityLevels.map(level => (
              <option key={level.id} value={level.id}>{level.title}</option>
            ))}
          </select>
          <label htmlFor="itemNote">Note</label>
          <textarea id="itemNote" rows={3} value={itemDetailsEditor.note || ''}
            onChange={e => setItemDetailsEditor(ed => ({ ...ed, note: e.target.value }))}></textarea>
          <footer>
            <button className="danger" style={{ marginInlineEnd: 'auto' }} onClick={() => deleteItem(selectedItem)}>Delete</button>
            <button className="secondary" onClick={closeItemDetailsDialog}>Cancel</button>
            <button onClick={updateItemDetails}>Update</button>
          </footer>
        </article>
      </dialog>
    </>
  );
}
