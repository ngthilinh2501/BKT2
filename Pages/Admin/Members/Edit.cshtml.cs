using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PCM_357.Data;
using PCM_357.Entities;

namespace PCM_357.Pages.Admin.Members
{
    [Authorize(Roles = "Admin")]
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly PCMContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public EditModel(PCMContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public Member Member { get; set; } = default!;

        [BindProperty]
        public List<string> SelectedRoles { get; set; } = new List<string>();

        public List<string> AvailableRoles { get; set; } = new List<string>();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Members == null)
            {
                return NotFound();
            }

            var member = await _context.Members.FirstOrDefaultAsync(m => m.Id == id);
            if (member == null)
            {
                return NotFound();
            }
            Member = member;

            // Load Roles
            AvailableRoles = await _roleManager.Roles.Select(r => r.Name!).ToListAsync();
            
            var user = await _userManager.FindByIdAsync(member.UserId);
            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                SelectedRoles = userRoles.ToList();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Note: We don't check ModelState for Roles specifically, but we should valid Member info
            // Fetch existing to prevent overposting read-only fields like Rank
            var memberToUpdate = await _context.Members.FindAsync(Member.Id);
            if (memberToUpdate == null)
            {
                return NotFound();
            }

            // Only update allowed fields
            memberToUpdate.FullName = Member.FullName;
            memberToUpdate.Email = Member.Email; // Note: Changing Email here does NOT auto-change Login Email. That's complex identity logic.
            // Requirement says "Edit", usually implies Profile info. Syncing Identity Email is extra, but let's assume just Profile for now or simple sync.
            memberToUpdate.PhoneNumber = Member.PhoneNumber;
            memberToUpdate.DateOfBirth = Member.DateOfBirth;
            memberToUpdate.IsActive = Member.IsActive;
            
            try
            {
                await _context.SaveChangesAsync();
                
                // Update Roles
                var user = await _userManager.FindByIdAsync(memberToUpdate.UserId);
                if (user != null)
                {
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    var rolesToAdd = SelectedRoles.Except(currentRoles).ToList();
                    var rolesToRemove = currentRoles.Except(SelectedRoles).ToList();

                    if (rolesToAdd.Any()) await _userManager.AddToRolesAsync(user, rolesToAdd);
                    if (rolesToRemove.Any()) await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MemberExists(Member.Id)) return NotFound();
                else throw;
            }

            return RedirectToPage("./Index");
        }

        private bool MemberExists(int id)
        {
            return (_context.Members?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
